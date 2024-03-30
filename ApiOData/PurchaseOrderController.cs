using Indotalent.Applications.PurchaseOrders;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class PurchaseOrderController : ODataController
    {
        private readonly PurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(PurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [EnableQuery]
        public IQueryable<PurchaseOrderDto> Get()
        {
            return _purchaseOrderService
                .GetAll()
                .Include(x => x.Vendor)
                .Include(x => x.Tax)
                .Select(rec => new PurchaseOrderDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    OrderDate = rec.OrderDate,
                    Status = rec.OrderStatus,
                    Description = rec.Description,
                    Vendor = rec.Vendor!.Name,
                    Tax = rec.Tax!.Name,
                    BeforeTaxAmount = rec.BeforeTaxAmount,
                    TaxAmount = rec.TaxAmount,
                    AfterTaxAmount = rec.AfterTaxAmount,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }



        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<PurchaseOrderDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_purchaseOrderService
                .GetAll()
                .Where(x => x.Id == key)
                .Select(rec => new PurchaseOrderDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    OrderDate = rec.OrderDate,
                    Status = rec.OrderStatus,
                    Description = rec.Description,
                    Vendor = rec.Vendor!.Name,
                    Tax = rec.Tax!.Name,
                    BeforeTaxAmount = rec.BeforeTaxAmount,
                    TaxAmount = rec.TaxAmount,
                    AfterTaxAmount = rec.AfterTaxAmount,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                })
            );
        }


    }
}
