using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class DeliveryOrderDto
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DeliveryOrderStatus? Status { get; set; }
        public string? SalesOrder { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Customer { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
