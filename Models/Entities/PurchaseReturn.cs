using Indotalent.Models.Contracts;
using Indotalent.Models.Enums;

namespace Indotalent.Models.Entities
{
    public class PurchaseReturn : _Base
    {
        public PurchaseReturn() { }
        public string? Number { get; set; }
        public DateTime? ReturnDate { get; set; }
        public PurchaseReturnStatus? Status { get; set; }
        public string? Description { get; set; }
        public required int GoodsReceiveId { get; set; }
        public GoodsReceive? GoodsReceive { get; set; }
    }
}
