using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.Scrappings;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoScrapping
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var scrappingService = services.GetRequiredService<ScrappingService>();
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();

            Random random = new Random();
            int scrappingStatusLength = Enum.GetNames(typeof(ScrappingStatus)).Length;
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
                    var scrapping = new Scrapping
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(Scrapping), "", "SCRP"),
                        ScrappingDate = transDate,
                        Status = (ScrappingStatus)random.Next(0, scrappingStatusLength),
                        WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                    };
                    await scrappingService.AddAsync(scrapping);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];

                        var inventoryTransaction = new InventoryTransaction
                        {
                            ModuleId = scrapping.Id,
                            ModuleName = nameof(Scrapping),
                            ModuleCode = "SCRP",
                            ModuleNumber = scrapping.Number,
                            MovementDate = scrapping.ScrappingDate.Value,
                            Status = (InventoryTransactionStatus)scrapping.Status,
                            Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                            WarehouseId = scrapping.WarehouseId,
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
