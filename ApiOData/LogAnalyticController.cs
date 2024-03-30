using Indotalent.Applications.LogAnalytics;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class LogAnalyticController : ODataController
    {
        private readonly LogAnalyticService _logAnalyticService;

        public LogAnalyticController(LogAnalyticService logAnalyticService)
        {
            _logAnalyticService = logAnalyticService;
        }

        [EnableQuery]
        public IQueryable<LogAnalyticDto> Get()
        {
            return _logAnalyticService
                .GetAll()
                .Select(rec => new LogAnalyticDto
                {
                    Id = rec.Id,
                    UserId = rec.UserId,
                    UserName = rec.UserName,
                    IPAddress = rec.IPAddress,
                    Url = rec.Url,
                    Device = rec.Device,
                    GeographicLocation = rec.GeographicLocation,
                    Browser = rec.Browser,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
