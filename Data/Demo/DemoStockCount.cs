using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.StockCounts;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoStockCount
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var stockCountService = services.GetRequiredService<StockCountService>();
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();

            Random random = new Random();
            int stockCountStatusLength = Enum.GetNames(typeof(StockCountStatus)).Length;
            var products = productService.GetAll().Where(x => x.Physical == true).ToList();
            var warehouses = warehouseService
                .GetAll()
                .Where(x => x.SystemWarehouse == false)
                .Select(x => x.Id).ToArray();

            var dateFinish = DateTime.Now;
            var dateStart = new DateTime(dateFinish.AddMonths(-12).Year, dateFinish.AddMonths(-12).Month, 1);

            for (DateTime date = dateStart; date < dateFinish; date = date.AddMonths(1))
            {
                DateTime[] transactionDates = DbInitializer.GetRandomDays(date.Year, date.Month, 6);

                foreach (DateTime transDate in transactionDates)
                {
                    var stockCount = new StockCount
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(StockCount), "", "SC"),
                        CountDate = transDate,
                        Status = (StockCountStatus)random.Next(0, stockCountStatusLength),
                        WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                    };
                    await stockCountService.AddAsync(stockCount);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];
                        var stock = inventoryTransactionService.GetStock(stockCount.WarehouseId, product.Id);
                        var qtyCount = stock + random.Next(-3, 6);

                        var inventoryTransaction = new InventoryTransaction
                        {
                            ModuleId = stockCount.Id,
                            ModuleName = nameof(StockCount),
                            ModuleCode = "Opname",
                            ModuleNumber = stockCount.Number,
                            MovementDate = stockCount.CountDate.Value,
                            Status = (InventoryTransactionStatus)stockCount.Status,
                            Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                            WarehouseId = stockCount.WarehouseId,
                            ProductId = product.Id,
                            Movement = random.Next(1, 10),
                            QtySCSys = stock,
                            QtySCCount = qtyCount > 0 ? qtyCount : 0,
                        };

                        inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                        await inventoryTransactionService.AddAsync(inventoryTransaction);
                    }


                }
            }
        }
    }
}
