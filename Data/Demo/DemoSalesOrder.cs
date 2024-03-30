using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.Applications.Taxes;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoSalesOrder
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var salesOrderService = services.GetRequiredService<SalesOrderService>();
            var salesOrderItemService = services.GetRequiredService<SalesOrderItemService>();
            var customerService = services.GetRequiredService<CustomerService>();
            var taxSerice = services.GetRequiredService<TaxService>();
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();

            Random random = new Random();
            int orderStatusLength = Enum.GetNames(typeof(SalesOrderStatus)).Length;
            var customers = customerService.GetAll().Select(x => x.Id).ToArray();
            var taxes = taxSerice.GetAll().Select(x => x.Id).ToArray();
            var products = productService.GetAll().ToList();

            var dateFinish = DateTime.Now;
            var dateStart = new DateTime(dateFinish.AddMonths(-12).Year, dateFinish.AddMonths(-12).Month, 1);

            for (DateTime date = dateStart; date < dateFinish; date = date.AddMonths(1))
            {
                DateTime[] transactionDates = DbInitializer.GetRandomDays(date.Year, date.Month, 6);

                foreach (DateTime transDate in transactionDates)
                {
                    var salesOrder = new SalesOrder
                    {
                        Number = numberSequenceService.GenerateNumber(nameof(SalesOrder), "", "SO"),
                        OrderDate = transDate,
                        OrderStatus = (SalesOrderStatus)random.Next(0, orderStatusLength),
                        CustomerId = DbInitializer.GetRandomValue(customers, random),
                        TaxId = DbInitializer.GetRandomValue(taxes, random),
                    };
                    await salesOrderService.AddAsync(salesOrder);

                    int numberOfProducts = random.Next(3, 6);
                    for (int i = 0; i < numberOfProducts; i++)
                    {
                        var product = products[random.Next(0, products.Count())];
                        var salesOrderItem = new SalesOrderItem
                        {
                            SalesOrderId = salesOrder.Id,
                            ProductId = product.Id,
                            Summary = product.Number,
                            UnitPrice = product.UnitPrice,
                            Quantity = random.Next(2, 5),
                        };
                        salesOrderItem.RecalculateTotal();
                        await salesOrderItemService.AddAsync(salesOrderItem);
                    }


                }
            }
        }
    }
}
