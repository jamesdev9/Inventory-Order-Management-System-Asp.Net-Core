using Indotalent.Applications.Warehouses;
using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Contracts;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Applications.InventoryTransactions
{
    public class InventoryTransactionService : Repository<InventoryTransaction>
    {
        private readonly WarehouseService _warehouseService;
        public InventoryTransactionService(
            WarehouseService warehouseService,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
            _warehouseService = warehouseService;
        }

        public override async Task AddAsync(InventoryTransaction? entity)
        {
            if (entity != null)
            {
                if (entity.QtySCCount < 0.0 && entity.ModuleName != nameof(StockCount))
                {
                    throw new Exception("Quantity count must not negatif");
                }

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.CreatedAtUtc = DateTime.Now;
                    auditEntity.CreatedByUserId = _userId;
                }

                CalculateInvenTrans(entity);

                _context.Set<InventoryTransaction>().Add(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public override async Task UpdateAsync(InventoryTransaction? entity)
        {
            if (entity != null)
            {
                if (entity.QtySCCount < 0.0 && entity.ModuleName != nameof(StockCount))
                {
                    throw new Exception("Quantity count must not negatif");
                }

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }


                CalculateInvenTrans(entity);

                _context.Set<InventoryTransaction>().Update(entity);
                await _context.SaveChangesAsync();

            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public double GetStock(int warehouseId, int productId)
        {
            var result = 0.0;
            result = GetAll()
                .Include(x => x.Product)
                .Where(x =>
                x.Status >= InventoryTransactionStatus.Confirmed &&
                x.WarehouseId == warehouseId &&
                x.ProductId == productId &&
                x.Product!.Physical == true)
                .Sum(x => x.Stock);
            return result;
        }

        public InventoryTransaction CalculateInvenTrans(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            var moduleName = transaction.ModuleName;

            switch (moduleName)
            {
                case nameof(DeliveryOrder):
                    DeliveryOrderProcessing(transaction);
                    break;
                case nameof(GoodsReceive):
                    GoodsReceiveProcessing(transaction);
                    break;
                case nameof(SalesReturn):
                    SalesReturnProcessing(transaction);
                    break;
                case nameof(PurchaseReturn):
                    PurchaseReturnProcessing(transaction);
                    break;
                case nameof(TransferIn):
                    TransferInProcessing(transaction);
                    break;
                case nameof(TransferOut):
                    TransferOutProcessing(transaction);
                    break;
                case nameof(StockCount):
                    StockCountProcessing(transaction);
                    break;
                case nameof(AdjustmentMinus):
                    AdjustmentMinusProcessing(transaction);
                    break;
                case nameof(AdjustmentPlus):
                    AdjustmentPlusProcessing(transaction);
                    break;
                case nameof(Scrapping):
                    ScrappingProcessing(transaction);
                    break;
                default:
                    break;
            }

            return transaction;
        }

        private InventoryTransaction DeliveryOrderProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.Out;
            transaction.CalculateStock();
            transaction.WarehouseFromId = transaction.WarehouseId;
            transaction.WarehouseToId = _warehouseService.GetCustomerWarehouse()!.Id;

            return transaction;
        }

        private InventoryTransaction GoodsReceiveProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.In;
            transaction.CalculateStock();
            transaction.WarehouseFromId = _warehouseService.GetVendorWarehouse()!.Id;
            transaction.WarehouseToId = transaction.WarehouseId;

            return transaction;
        }

        private InventoryTransaction SalesReturnProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.In;
            transaction.CalculateStock();
            transaction.WarehouseFromId = _warehouseService.GetVendorWarehouse()!.Id;
            transaction.WarehouseToId = transaction.WarehouseId;

            return transaction;
        }

        private InventoryTransaction PurchaseReturnProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.Out;
            transaction.CalculateStock();
            transaction.WarehouseFromId = transaction.WarehouseId;
            transaction.WarehouseToId = _warehouseService.GetCustomerWarehouse()!.Id;

            return transaction;
        }

        private InventoryTransaction TransferInProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.In;
            transaction.CalculateStock();
            transaction.WarehouseFromId = _warehouseService.GetTransferWarehouse()!.Id;
            transaction.WarehouseToId = transaction.WarehouseId;

            return transaction;
        }

        private InventoryTransaction TransferOutProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.Out;
            transaction.CalculateStock();
            transaction.WarehouseFromId = transaction.WarehouseId;
            transaction.WarehouseToId = _warehouseService.GetTransferWarehouse()!.Id;

            return transaction;
        }

        private InventoryTransaction StockCountProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.CalculateStockCountDelta();

            if (transaction.QtySCDelta < 0.0)
            {

                transaction.TransType = InventoryTransType.In;
                transaction.CalculateStock();
                transaction.WarehouseFromId = _warehouseService.GetStockCountWarehouse()!.Id;
                transaction.WarehouseToId = transaction.WarehouseId;

            }
            else
            {

                transaction.TransType = InventoryTransType.Out;
                transaction.CalculateStock();
                transaction.WarehouseFromId = transaction.WarehouseId;
                transaction.WarehouseToId = _warehouseService.GetStockCountWarehouse()!.Id;

            }

            return transaction;
        }

        private InventoryTransaction AdjustmentMinusProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.Out;
            transaction.CalculateStock();
            transaction.WarehouseFromId = transaction.WarehouseId;
            transaction.WarehouseToId = _warehouseService.GetAdjustmentWarehouse()!.Id;

            return transaction;
        }

        private InventoryTransaction AdjustmentPlusProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.In;
            transaction.CalculateStock();
            transaction.WarehouseFromId = _warehouseService.GetAdjustmentWarehouse()!.Id;
            transaction.WarehouseToId = transaction.WarehouseId;

            return transaction;
        }

        private InventoryTransaction ScrappingProcessing(InventoryTransaction transaction)
        {
            if (transaction == null)
            {
                throw new Exception("Inventory transaction is null");
            }

            transaction.TransType = InventoryTransType.Out;
            transaction.CalculateStock();
            transaction.WarehouseFromId = transaction.WarehouseId;
            transaction.WarehouseToId = _warehouseService.GetScrappingWarehouse()!.Id;

            return transaction;
        }


    }
}
