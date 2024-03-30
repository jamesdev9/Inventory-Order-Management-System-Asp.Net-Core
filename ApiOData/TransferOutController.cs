using Indotalent.Applications.TransferOuts;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class TransferOutController : ODataController
    {
        private readonly TransferOutService _transferOutService;

        public TransferOutController(TransferOutService transferOutService)
        {
            _transferOutService = transferOutService;
        }

        [EnableQuery]
        public IQueryable<TransferOutDto> Get()
        {
            return _transferOutService
                .GetAll()
                .Include(x => x.WarehouseFrom)
                .Include(x => x.WarehouseTo)
                .Select(rec => new TransferOutDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    TransferReleaseDate = rec.TransferReleaseDate,
                    Status = rec.Status,
                    WarehouseFrom = rec.WarehouseFrom!.Name,
                    WarehouseTo = rec.WarehouseTo!.Name,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }




    }
}
