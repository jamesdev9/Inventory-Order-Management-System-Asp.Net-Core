using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Entities;
using System.Security.Claims;

namespace Indotalent.Applications.LogSessions
{
    public class LogSessionService : Repository<LogSession>
    {
        public LogSessionService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }

        public async Task CollectLoginSessionDataAsync()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var data = new LogSession
            {
                UserId = userId,
                UserName = userName,
                IPAddress = ipAddress,
                Action = "Login"
            };

            await AddAsync(data);
        }

        public async Task CollectLogoutSessionDataAsync()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var data = new LogSession
            {
                UserId = userId,
                UserName = userName,
                IPAddress = ipAddress,
                Action = "Logout"
            };

            await AddAsync(data);
        }

        public void PurgeAllData()
        {
            _context.LogSession.RemoveRange(_context.LogSession);
            _context.SaveChanges();
        }
    }
}
