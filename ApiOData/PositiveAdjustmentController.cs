using Indotalent.Applications.AdjustmentPluss;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class PositiveAdjustmentController : ODataController
    {
        private readonly AdjustmentPlusService _adjustmentPlusService;

        public PositiveAdjustmentController(AdjustmentPlusService adjustmentPlusService)
        {
            _adjustmentPlusService = adjustmentPlusService;
        }

        [EnableQuery]
        public IQueryable<PositiveAdjustmentDto> Get()
        {
            return _adjustmentPlusService
                .GetAll()
                .Select(rec => new PositiveAdjustmentDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    AdjustmentDate = rec.AdjustmentDate,
                    Status = rec.Status,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }




    }
}
