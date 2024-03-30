namespace Indotalent.DTOs
{
    public class PurchaseOrderItemChildDto
    {
        public int? Id { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? ProductId { get; set; }
        public string? Summary { get; set; }
        public double? UnitPrice { get; set; }
        public double? Quantity { get; set; }
        public double? Total { get; set; }

    }
}
