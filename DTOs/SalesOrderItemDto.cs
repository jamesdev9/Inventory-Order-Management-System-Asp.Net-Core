namespace Indotalent.DTOs
{
    public class SalesOrderItemDto
    {
        public int? Id { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public string? SalesOrder { get; set; }
        public string? Customer { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Product { get; set; }
        public string? Summary { get; set; }
        public double? UnitPrice { get; set; }
        public double? Quantity { get; set; }
        public double? Total { get; set; }
    }
}
