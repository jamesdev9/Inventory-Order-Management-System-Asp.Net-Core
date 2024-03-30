using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class PurchaseReturnDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? ReturnDate { get; set; }
        public PurchaseReturnStatus? Status { get; set; }
        public string? GoodsReceive { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? Vendor { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
