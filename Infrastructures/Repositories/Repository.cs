using Indotalent.Data;
using Indotalent.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Indotalent.Infrastructures.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IHasId, IHasAudit, IHasSoftDelete
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IAuditColumnTransformer _auditColumnTransformer;
        protected readonly string _userId = string.Empty;
        protected readonly string _userName = string.Empty;

        public Repository(
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
        private static string GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        private static string GetUserName(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public virtual IQueryable<T> GetAllArchive()
        {
            return _context.Set<T>()
                .ApplyIsDeletedFilter()
                .AsNoTracking();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>()
                .ApplyIsNotDeletedFilter()
                .AsNoTracking();
        }

        public virtual async Task<T?> GetByIdAsync(int? id)
        {
            if (!id.HasValue)
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                await _auditColumnTransformer.TransformAsync(entity, _context);
            }

            return entity;
        }

        public virtual async Task<T?> GetByRowGuidAsync(Guid? rowGuid)
        {
            if (!rowGuid.HasValue)
            {
                throw new Exception("Unable to process, row guid is null");
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.RowGuid == rowGuid);

            if (entity != null)
            {
                await _auditColumnTransformer.TransformAsync(entity, _context);
            }

            return entity;
        }

        public virtual async Task AddAsync(T? entity)
        {
            if (entity != null)
            {
                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.CreatedAtUtc = DateTime.Now;
                    auditEntity.CreatedByUserId = _userId;
                }
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task UpdateAsync(T? entity)
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
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task DeleteByIdAsync(int? id)
        {
            if (!id.HasValue)
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<T>()
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
                    _context.Set<T>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteByRowGuidAsync(Guid? rowGuid)
        {
            if (!rowGuid.HasValue)
            {
                throw new Exception("Unable to process, row guid is null");
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.RowGuid == rowGuid);

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
                    _context.Set<T>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
