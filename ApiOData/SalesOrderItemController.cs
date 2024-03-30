using Indotalent.Applications.SalesOrderItems;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class SalesOrderItemController : ODataController
    {
        private readonly SalesOrderItemService _salesOrderItemService;

        public SalesOrderItemController(SalesOrderItemService salesOrderItemService)
        {
            _salesOrderItemService = salesOrderItemService;
        }

        [EnableQuery]
        public IQueryable<SalesOrderItemDto> Get()
        {
            return _salesOrderItemService
                .GetAll()
                .Include(x => x.SalesOrder)
                    .ThenInclude(x => x!.Customer)
                .Include(x => x.Product)
                .Select(rec => new SalesOrderItemDto
                {
                    Id = rec.Id,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                    SalesOrder = rec.SalesOrder!.Number,
                    Customer = rec.SalesOrder.Customer!.Name,
                    Product = rec.Product!.Name,
                    Summary = rec.Summary,
                    UnitPrice = rec.UnitPrice,
                    Quantity = rec.Quantity,
                    Total = rec.Total,
                    OrderDate = rec.SalesOrder!.OrderDate
                });
        }


    }
}
