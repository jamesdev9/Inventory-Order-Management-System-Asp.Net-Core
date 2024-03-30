using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.TransferOuts;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoTransferOut
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var transferOutService = services.GetRequiredService<TransferOutService>();
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();

            Random random = new Random();
            int transferStatusLength = Enum.GetNames(typeof(TransferStatus)).Length;
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
                    var fromId = DbInitializer.GetRandomValue(warehouses, random);
                    warehouses = warehouses.Where((val, idx) => idx != fromId).ToArray();
                    var toId = DbInitializer.GetRandomValue(warehouses, random);
                    var transferOut = new TransferOut
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(TransferOut), "", "OUT"),
                        TransferReleaseDate = transDate,
                        Status = (TransferStatus)random.Next(0, transferStatusLength),
                        WarehouseFromId = fromId,
                        WarehouseToId = toId,
                    };
                    await transferOutService.AddAsync(transferOut);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];

                        var inventoryTransaction = new InventoryTransaction
                        {
                            ModuleId = transferOut.Id,
                            ModuleName = nameof(TransferOut),
                            ModuleCode = "TO-OUT",
                            ModuleNumber = transferOut.Number,
                            MovementDate = transferOut.TransferReleaseDate.Value,
                            Status = (InventoryTransactionStatus)transferOut.Status,
                            Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                            WarehouseId = transferOut.WarehouseFromId.Value,
                            ProductId = product.Id,
                            Movement = random.Next(1, 10)
                        };

                        inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                        await inventoryTransactionService.AddAsync(inventoryTransaction);
                    }


                }
            }
        }
    }
}
