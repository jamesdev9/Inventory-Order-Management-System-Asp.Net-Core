using Indotalent.Applications.LogSessions;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class LogSessionController : ODataController
    {
        private readonly LogSessionService _logErrorService;

        public LogSessionController(LogSessionService logErrorService)
        {
            _logErrorService = logErrorService;
        }

        [EnableQuery]
        public IQueryable<LogSessionDto> Get()
        {
            return _logErrorService
                .GetAll()
                .Select(rec => new LogSessionDto
                {
                    Id = rec.Id,
                    UserId = rec.UserId,
                    UserName = rec.UserName,
                    IPAddress = rec.IPAddress,
                    Action = rec.Action,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
