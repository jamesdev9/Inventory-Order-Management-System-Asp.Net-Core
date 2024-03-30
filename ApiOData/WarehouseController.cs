using Indotalent.Applications.Warehouses;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class WarehouseController : ODataController
    {
        private readonly WarehouseService _warehouseService;

        public WarehouseController(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [EnableQuery]
        public IQueryable<WarehouseDto> Get()
        {
            return _warehouseService
                .GetAll()
                .Select(rec => new WarehouseDto
                {
                    Id = rec.Id,
                    RowGuid = rec.RowGuid,
                    Name = rec.Name,
                    SystemWarehouse = rec.SystemWarehouse,
                    Description = rec.Description,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
