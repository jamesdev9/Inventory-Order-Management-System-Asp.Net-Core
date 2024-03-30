using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class ScrappingDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? ScrappingDate { get; set; }
        public ScrappingStatus? Status { get; set; }
        public string? Warehouse { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
