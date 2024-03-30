using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class SalesOrderDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? OrderDate { get; set; }
        public SalesOrderStatus? Status { get; set; }
        public string? Description { get; set; }
        public string? Customer { get; set; }
        public string? Tax { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public double? BeforeTaxAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double? AfterTaxAmount { get; set; }
    }
}
