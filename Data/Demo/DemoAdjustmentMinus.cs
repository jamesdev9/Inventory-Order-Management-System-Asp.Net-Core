using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoAdjustmentMinus
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var adjustmentMinusService = services.GetRequiredService<AdjustmentMinusService>();
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
                    var adjustmentMinus = new AdjustmentMinus
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(AdjustmentMinus), "", "ADJ-"),
                        AdjustmentDate = transDate,
                        Status = (AdjustmentStatus)random.Next(0, adjustmentStatusLength),
                    };
                    await adjustmentMinusService.AddAsync(adjustmentMinus);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];

                        var inventoryTransaction = new InventoryTransaction
                        {
                            ModuleId = adjustmentMinus.Id,
                            ModuleName = nameof(AdjustmentMinus),
                            ModuleCode = "ADJ-",
                            ModuleNumber = adjustmentMinus.Number,
                            MovementDate = adjustmentMinus.AdjustmentDate.Value,
                            Status = (InventoryTransactionStatus)adjustmentMinus.Status,
                            Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                            WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                            ProductId = product.Id,
                            Movement = random.Next(1, 3)
                        };

                        inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                        await inventoryTransactionService.AddAsync(inventoryTransaction);
                    }


                }
            }
        }
    }
}
