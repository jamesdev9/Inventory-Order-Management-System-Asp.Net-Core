using Indotalent.Applications.InventoryTransactions;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class InvenStockController : ODataController
    {
        private readonly InventoryTransactionService _transactionService;

        public InvenStockController(InventoryTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [EnableQuery]
        public IQueryable<InvenStockDto> Get()
        {
            var transGrouped = _transactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Where(x =>
                    x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed &&
                    x.Warehouse!.SystemWarehouse == false &&
                    x.Product!.Physical == true
                )
                .GroupBy(x => new { x.WarehouseId, x.ProductId })
                .Select(group => new InvenStockDto
                {
                    WarehouseId = group.Key.WarehouseId,
                    ProductId = group.Key.ProductId,
                    Warehouse = group.Max(x => x.Warehouse!.Name),
                    Product = group.Max(x => x.Product!.Name),
                    Stock = group.Sum(x => x.Stock),
                    Id = group.Max(x => x.Id),
                    RowGuid = group.Max(x => x.RowGuid),
                    CreatedAtUtc = group.Max(x => x.CreatedAtUtc)
                });


            return transGrouped;
        }


    }
}
