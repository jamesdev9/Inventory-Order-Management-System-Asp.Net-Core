using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class SalesOrderItem : _Base
    {
        public SalesOrderItem() { }
        public required int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }
        public required int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? Summary { get; set; }
        public double? UnitPrice { get; set; } = 0;
        public double? Quantity { get; set; } = 1;
        public double? Total { get; set; } = 0;

        public void RecalculateTotal()
        {
            Total = Quantity * UnitPrice;
        }
    }
}
