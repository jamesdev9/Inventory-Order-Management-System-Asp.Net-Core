using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.SalesReturns;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoSalesReturn
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var salesReturnService = services.GetRequiredService<SalesReturnService>();
            var deliveryOrderService = services.GetRequiredService<DeliveryOrderService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();
            Random random = new Random();
            int salesReturnStatusLength = Enum.GetNames(typeof(SalesReturnStatus)).Length;

            var deliveryOrders = deliveryOrderService
                .GetAll()
                .Where(x => x.Status >= DeliveryOrderStatus.Confirmed)
                .ToList();

            var warehouses = warehouseService
                .GetAll()
                .Where(x => x.SystemWarehouse == false)
                .Select(x => x.Id)
                .ToArray();

            foreach (var deliveryOrder in deliveryOrders)
            {
                bool process = random.Next(2) == 0;

                if (process)
                {
                    continue;
                }

                var salesReturn = new SalesReturn
                {
                    Number = numberSequenceService.GenerateNumber(nameof(SalesReturn), "", "SRN"),
                    ReturnDate = deliveryOrder.DeliveryDate?.AddDays(random.Next(1, 5)),
                    Status = (SalesReturnStatus)random.Next(0, salesReturnStatusLength),
                    DeliveryOrderId = deliveryOrder.Id,
                };
                await salesReturnService.AddAsync(salesReturn);

                var items = inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == deliveryOrder.Id && x.ModuleName == nameof(DeliveryOrder))
                    .ToList();

                foreach (var item in items)
                {
                    var inventoryTransaction = new InventoryTransaction
                    {
                        ModuleId = salesReturn.Id,
                        ModuleName = nameof(SalesReturn),
                        ModuleCode = "SRN",
                        ModuleNumber = salesReturn.Number,
                        MovementDate = salesReturn.ReturnDate!.Value,
                        Status = (InventoryTransactionStatus)salesReturn.Status,
                        Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                        WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                        ProductId = item.ProductId,
                        Movement = item.Movement
                    };

                    inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                    await inventoryTransactionService.AddAsync(inventoryTransaction);
                }


            }

        }
    }
}
