using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Data.Demo
{
    public static class DemoDeliveryOrder
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var deliveryOrderService = services.GetRequiredService<DeliveryOrderService>();
            var salesOrderService = services.GetRequiredService<SalesOrderService>();
            var salesOrderItemService = services.GetRequiredService<SalesOrderItemService>();
            var warehouseService = services.GetRequiredService<WarehouseService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();
            Random random = new Random();
            int deliveryOrderStatusLength = Enum.GetNames(typeof(DeliveryOrderStatus)).Length;

            var salesOrders = salesOrderService
                .GetAll()
                .Where(x => x.OrderStatus >= SalesOrderStatus.Confirmed)
                .ToList();

            var warehouses = warehouseService
                .GetAll()
                .Where(x => x.SystemWarehouse == false)
                .Select(x => x.Id)
                .ToArray();

            foreach (var salesOrder in salesOrders)
            {
                var deliveryOrder = new DeliveryOrder
                {
                    Number = numberSequenceService.GenerateNumber(nameof(DeliveryOrder), "", "DO"),
                    DeliveryDate = salesOrder.OrderDate?.AddDays(random.Next(1, 5)),
                    Status = (DeliveryOrderStatus)random.Next(0, deliveryOrderStatusLength),
                    SalesOrderId = salesOrder.Id,
                };
                await deliveryOrderService.AddAsync(deliveryOrder);

                var items = salesOrderItemService
                    .GetAll()
                    .Include(x => x.Product)
                    .Where(x => x.SalesOrderId == salesOrder.Id && x.Product!.Physical == true).ToList();

                foreach (var item in items)
                {
                    var inventoryTransaction = new InventoryTransaction
                    {
                        ModuleId = deliveryOrder.Id,
                        ModuleName = nameof(DeliveryOrder),
                        ModuleCode = "DO",
                        ModuleNumber = deliveryOrder.Number,
                        MovementDate = deliveryOrder.DeliveryDate!.Value,
                        Status = (InventoryTransactionStatus)deliveryOrder.Status,
                        Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                        WarehouseId = DbInitializer.GetRandomValue(warehouses, random),
                        ProductId = item.ProductId,
                        Movement = item.Quantity!.Value
                    };

                    inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                    await inventoryTransactionService.AddAsync(inventoryTransaction);
                }


            }

        }
    }
}
