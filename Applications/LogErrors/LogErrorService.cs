using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Entities;

namespace Indotalent.Applications.LogErrors
{
    public class LogErrorService : Repository<LogError>
    {
        public LogErrorService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }

        public async Task CollectErrorDataAsync(string? exceptionMessage, string? stackTrace, string? additionalInfo)
        {
            var data = new LogError
            {
                ExceptionMessage = exceptionMessage,
                StackTrace = stackTrace,
                AdditionalInfo = additionalInfo
            };

            await AddAsync(data);
        }

        public void PurgeAllData()
        {
            _context.LogError.RemoveRange(_context.LogError);
            _context.SaveChanges();
        }

    }
}
