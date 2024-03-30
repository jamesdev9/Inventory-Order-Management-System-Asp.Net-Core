using Indotalent.Applications.TransferIns;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class TransferInController : ODataController
    {
        private readonly TransferInService _transferInService;

        public TransferInController(TransferInService transferInService)
        {
            _transferInService = transferInService;
        }

        [EnableQuery]
        public IQueryable<TransferInDto> Get()
        {
            return _transferInService
                .GetAll()
                .Include(x => x.TransferOut)
                    .ThenInclude(x => x!.WarehouseFrom)
                .Include(x => x.TransferOut)
                    .ThenInclude(x => x!.WarehouseTo)
                .Select(rec => new TransferInDto
                {
                    Id = rec.Id,
                    Number = rec.Number,
                    ReceiveDate = rec.TransferReceiveDate,
                    Status = rec.Status,
                    TransferOut = rec.TransferOut!.Number,
                    ReleaseDate = rec.TransferOut!.TransferReleaseDate,
                    WarehouseFrom = rec.TransferOut!.WarehouseFrom!.Name,
                    WarehouseTo = rec.TransferOut!.WarehouseTo!.Name,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                });
        }




    }
}
