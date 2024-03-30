using DeviceDetectorNET;
using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Entities;
using System.Security.Claims;
using UAParser;

namespace Indotalent.Applications.LogAnalytics
{
    public class LogAnalyticService : Repository<LogAnalytic>
    {
        public LogAnalyticService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }

        public async Task CollectAnalyticDataAsync()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userAgentString = _httpContextAccessor?.HttpContext?.Request.Headers["User-Agent"];
            var userIpAddress = _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var url = _httpContextAccessor?.HttpContext?.Request.Path;

            var deviceDetector = new DeviceDetector(userAgentString);
            deviceDetector.Parse();
            var deviceType = deviceDetector.GetDeviceName();

            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgentString);
            var browserName = clientInfo?.UA?.Family;
            var browserVersion = clientInfo?.UA?.Major;

            var logAnalytic = new LogAnalytic
            {
                UserId = userId,
                UserName = userName,
                IPAddress = userIpAddress,
                Url = url,
                Device = deviceType,
                GeographicLocation = "",
                Browser = $"{browserName} {browserVersion}"
            };

            await AddAsync(logAnalytic);
        }

        public void PurgeAllData()
        {
            _context.LogAnalytic.RemoveRange(_context.LogAnalytic);
            _context.SaveChanges();
        }

    }
}
