namespace Indotalent.DTOs
{
    public class PurchaseOrderItemDto
    {
        public int? Id { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public string? PurchaseOrder { get; set; }
        public string? Vendor { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Product { get; set; }
        public string? Summary { get; set; }
        public double? UnitPrice { get; set; }
        public double? Quantity { get; set; }
        public double? Total { get; set; }
    }
}
