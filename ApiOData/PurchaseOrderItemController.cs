using Indotalent.Applications.PurchaseOrderItems;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class PurchaseOrderItemController : ODataController
    {
        private readonly PurchaseOrderItemService _purchaseOrderItemService;

        public PurchaseOrderItemController(PurchaseOrderItemService purchaseOrderItemService)
        {
            _purchaseOrderItemService = purchaseOrderItemService;
        }

        [EnableQuery]
        public IQueryable<PurchaseOrderItemDto> Get()
        {
            return _purchaseOrderItemService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x!.Vendor)
                .Include(x => x.Product)
                .Select(rec => new PurchaseOrderItemDto
                {
                    Id = rec.Id,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                    PurchaseOrder = rec.PurchaseOrder!.Number,
                    Vendor = rec.PurchaseOrder.Vendor!.Name,
                    Product = rec.Product!.Name,
                    Summary = rec.Summary,
                    UnitPrice = rec.UnitPrice,
                    Quantity = rec.Quantity,
                    Total = rec.Total,
                    OrderDate = rec.PurchaseOrder!.OrderDate
                });
        }


    }
}
