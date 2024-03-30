using Indotalent.Applications.SalesReturns;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class SalesReturnController : ODataController
    {
        private readonly SalesReturnService _salesReturnService;

        public SalesReturnController(SalesReturnService salesReturnService)
        {
            _salesReturnService = salesReturnService;
        }

        [EnableQuery]
        public IQueryable<SalesReturnDto> Get()
        {
            return _salesReturnService
                .GetAll()
                .Include(x => x.DeliveryOrder)
                    .ThenInclude(x => x!.SalesOrder)
                        .ThenInclude(x => x!.Customer)
                .Select(rec => new SalesReturnDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    ReturnDate = rec.ReturnDate,
                    Status = rec.Status,
                    DeliveryOrder = rec.DeliveryOrder!.Number,
                    DeliveryDate = rec.DeliveryOrder!.DeliveryDate,
                    Customer = rec.DeliveryOrder!.SalesOrder!.Customer!.Name,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }




    }
}
