using Indotalent.Models.Enums;

namespace Indotalent.DTOs
{
    public class InvenTransDto
    {
        public int Id { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleNumber { get; set; }
        public DateTime MovementDate { get; set; }
        public InventoryTransactionStatus Status { get; set; }
        public string? Number { get; set; }
        public string? Warehouse { get; set; }
        public string? Product { get; set; }
        public double Movement { get; set; }
        public InventoryTransType TransType { get; set; }
        public double Stock { get; set; }
        public string? WarehouseFrom { get; set; }
        public string? WarehouseTo { get; set; }
    }
}
