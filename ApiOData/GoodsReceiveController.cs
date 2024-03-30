using Indotalent.Applications.GoodsReceives;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class GoodsReceiveController : ODataController
    {
        private readonly GoodsReceiveService _goodsReceiveService;

        public GoodsReceiveController(GoodsReceiveService goodsReceiveService)
        {
            _goodsReceiveService = goodsReceiveService;
        }

        [EnableQuery]
        public IQueryable<GoodsReceiveDto> Get()
        {
            return _goodsReceiveService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x!.Vendor)
                .Select(rec => new GoodsReceiveDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    ReceiveDate = rec.ReceiveDate,
                    Status = rec.Status,
                    PurchaseOrder = rec.PurchaseOrder!.Number,
                    OrderDate = rec.PurchaseOrder!.OrderDate,
                    Vendor = rec.PurchaseOrder!.Vendor!.Name,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }




    }
}
