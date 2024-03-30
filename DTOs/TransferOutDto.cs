using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class TransferOutDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? TransferReleaseDate { get; set; }
        public TransferStatus? Status { get; set; }
        public string? WarehouseFrom { get; set; }
        public string? WarehouseTo { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
