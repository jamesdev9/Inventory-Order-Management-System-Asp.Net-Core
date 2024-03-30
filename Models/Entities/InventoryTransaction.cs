using Indotalent.Models.Contracts;
using Indotalent.Models.Enums;

namespace Indotalent.Models.Entities
{
    public class InventoryTransaction : _Base
    {
        public InventoryTransaction()
        {
            this.QtySCSys = 0;
            this.QtySCCount = 0;
            this.QtySCDelta = 0;
        }
        public required int ModuleId { get; set; }
        public required string ModuleName { get; set; }
        public required string ModuleCode { get; set; }
        public required string ModuleNumber { get; set; }
        public required DateTime MovementDate { get; set; }
        public required InventoryTransactionStatus Status { get; set; }
        public string? Number { get; set; }
        public required int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public required int ProductId { get; set; }
        public Product? Product { get; set; }
        public double Movement { get; set; }
        public InventoryTransType TransType { get; set; }
        public double Stock { get; set; }
        public int WarehouseFromId { get; set; }
        public Warehouse? WarehouseFrom { get; set; }
        public int WarehouseToId { get; set; }
        public Warehouse? WarehouseTo { get; set; }
        public double QtySCSys { get; set; }
        public double QtySCCount { get; set; }
        public double QtySCDelta { get; set; }

        public void CalculateStockCountDelta()
        {
            this.QtySCDelta = this.QtySCSys - this.QtySCCount;
            this.Movement = Math.Abs(this.QtySCDelta);
        }

        public void CalculateStock()
        {
            this.Stock = this.Movement * (int)this.TransType;
        }

    }
}
