using Indotalent.Applications.AdjustmentPluss;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoAdjustmentPlus
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var adjustmentPlusService = services.GetRequiredService<AdjustmentPlusService>();
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();

            Random random = new Random();
            int adjustmentStatusLength = Enum.GetNames(typeof(AdjustmentStatus)).Length;
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
                    var adjustmentPlus = new AdjustmentPlus
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(AdjustmentPlus), "", "ADJ+"),
                        AdjustmentDate = transDate,
                        Status = (AdjustmentStatus)random.Next(0, adjustmentStatusLength),
                    };
                    await adjustmentPlusService.AddAsync(adjustmentPlus);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];

                        var inventoryTransaction = new InventoryTransaction
                        {
                            ModuleId = adjustmentPlus.Id,
                            ModuleName = nameof(AdjustmentPlus),
                            ModuleCode = "ADJ+",
                            ModuleNumber = adjustmentPlus.Number,
                            MovementDate = adjustmentPlus.AdjustmentDate.Value,
                            Status = (InventoryTransactionStatus)adjustmentPlus.Status,
                            Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                            WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                            ProductId = product.Id,
                            Movement = random.Next(5, 10)
                        };

                        inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                        await inventoryTransactionService.AddAsync(inventoryTransaction);
                    }


                }
            }
        }
    }
}
