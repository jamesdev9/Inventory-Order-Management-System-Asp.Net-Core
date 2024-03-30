using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class NegativeAdjustmentController : ODataController
    {
        private readonly AdjustmentMinusService _adjustmentMinusService;

        public NegativeAdjustmentController(AdjustmentMinusService adjustmentMinusService)
        {
            _adjustmentMinusService = adjustmentMinusService;
        }

        [EnableQuery]
        public IQueryable<NegativeAdjustmentDto> Get()
        {
            return _adjustmentMinusService
                .GetAll()
                .Select(rec => new NegativeAdjustmentDto
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
