using Indotalent.Models.Contracts;
using Indotalent.Models.Enums;

namespace Indotalent.Models.Entities
{
    public class SalesOrder : _Base
    {
        public SalesOrder() { }
        public string? Number { get; set; }
        public DateTime? OrderDate { get; set; }
        public SalesOrderStatus? OrderStatus { get; set; }
        public string? Description { get; set; }
        public required int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public required int TaxId { get; set; }
        public Tax? Tax { get; set; }
        public double? BeforeTaxAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double? AfterTaxAmount { get; set; }
    }
}
