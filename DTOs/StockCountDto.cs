using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class StockCountDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? CountDate { get; set; }
        public StockCountStatus? Status { get; set; }
        public string? Warehouse { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
