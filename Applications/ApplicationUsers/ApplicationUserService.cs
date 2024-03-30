using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Contracts;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Indotalent.Applications.ApplicationUsers
{
    public class ApplicationUserService
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IAuditColumnTransformer _auditColumnTransformer;
        protected readonly string _userId = string.Empty;
        protected readonly string _userName = string.Empty;

        public ApplicationUserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditColumnTransformer = auditColumnTransformer;
            _userId = GetUserId(_httpContextAccessor);
            _userName = GetUserName(_httpContextAccessor);
        }

        public bool IsEmailAlreadyExist(string? email)
        {
            var result = true;
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                result = false;
            }
            return result;
        }

        public string? GetAvatarId(string? userId)
        {
            var result = string.Empty;
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                result = user.Avatar;
            }
            return result;
        }
        private static string GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        private static string GetUserName(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public virtual IQueryable<ApplicationUser> GetAllArchive()
        {
            return _context.Set<ApplicationUser>()
                .ApplyIsDeletedFilter()
                .AsNoTracking();
        }

        public virtual IQueryable<ApplicationUser> GetAll()
        {
            return _context.Set<ApplicationUser>()
                .ApplyIsNotDeletedFilter()
                .AsNoTracking();
        }

        public virtual async Task<ApplicationUser?> GetByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<ApplicationUser>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                await _auditColumnTransformer.TransformAsync(entity, _context);
            }

            return entity;
        }

        public virtual async Task AddAsync(ApplicationUser? entity)
        {
            if (entity != null)
            {
                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.CreatedByUserId = _userId;
                }
                _context.Set<ApplicationUser>().Add(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task UpdateAsync(ApplicationUser? entity)
        {
            if (entity != null)
            {
                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }
                _context.Set<ApplicationUser>().Update(entity);
                await _context.SaveChangesAsync();

            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task DeleteByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<ApplicationUser>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }

                if (entity is IHasSoftDelete softDeleteEntity)
                {
                    softDeleteEntity.IsNotDeleted = false;
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Set<ApplicationUser>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }


    }
}
