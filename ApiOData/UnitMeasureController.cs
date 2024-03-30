using Indotalent.Applications.UnitMeasures;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class UnitMeasureController : ODataController
    {
        private readonly UnitMeasureService _unitMeasureService;

        public UnitMeasureController(UnitMeasureService unitMeasureService)
        {
            _unitMeasureService = unitMeasureService;
        }

        [EnableQuery]
        public IQueryable<UnitMeasureDto> Get()
        {
            return _unitMeasureService
                .GetAll()
                .Select(rec => new UnitMeasureDto
                {
                    Id = rec.Id,
                    RowGuid = rec.RowGuid,
                    Name = rec.Name,
                    Description = rec.Description,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
