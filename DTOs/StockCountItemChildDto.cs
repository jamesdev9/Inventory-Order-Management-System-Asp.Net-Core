namespace Indotalent.DTOs
{
    public class StockCountItemChildDto
    {
        public int? Id { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public int? ProductId { get; set; }
        public double? QtySCSys { get; set; }
        public double? QtySCCount { get; set; }
        public double? Stock { get; set; }
    }
}
