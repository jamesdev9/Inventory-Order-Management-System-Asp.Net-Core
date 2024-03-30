using Indotalent.Applications.PurchaseOrders;
using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Contracts;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Applications.PurchaseOrderItems
{
    public class PurchaseOrderItemService : Repository<PurchaseOrderItem>
    {
        private readonly PurchaseOrderService _purchaseOrderService;

        public PurchaseOrderItemService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer,
            PurchaseOrderService purchaseOrderService) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        public override async Task AddAsync(PurchaseOrderItem? entity)
        {
            if (entity != null)
            {
                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.CreatedAtUtc = DateTime.Now;
                    auditEntity.CreatedByUserId = _userId;
                }
                entity.RecalculateTotal();
                _context.Set<PurchaseOrderItem>().Add(entity);
                await _context.SaveChangesAsync();

                await _purchaseOrderService.RecalculateParentAsync(entity.PurchaseOrderId);
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public override async Task UpdateAsync(PurchaseOrderItem? entity)
        {
            if (entity != null)
            {
                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }
                entity.RecalculateTotal();
                _context.Set<PurchaseOrderItem>().Update(entity);
                await _context.SaveChangesAsync();


                await _purchaseOrderService.RecalculateParentAsync(entity.PurchaseOrderId);
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }


        public override async Task DeleteByIdAsync(int? id)
        {
            if (!id.HasValue)
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<PurchaseOrderItem>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }

                if (entity is IHasSoftDelete softDeleteEntity)
                {
                    softDeleteEntity.IsNotDeleted = false;
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Set<PurchaseOrderItem>().Remove(entity);
                }

                await _context.SaveChangesAsync();


                await _purchaseOrderService.RecalculateParentAsync(entity.PurchaseOrderId);
            }
        }

        public override async Task DeleteByRowGuidAsync(Guid? rowGuid)
        {
            if (!rowGuid.HasValue)
            {
                throw new Exception("Unable to process, row guid is null");
            }

            var entity = await _context.Set<PurchaseOrderItem>()
                .FirstOrDefaultAsync(x => x.RowGuid == rowGuid);

            if (entity != null)
            {

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }

                if (entity is IHasSoftDelete softDeleteEntity)
                {
                    softDeleteEntity.IsNotDeleted = false;
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Set<PurchaseOrderItem>().Remove(entity);
                }

                await _context.SaveChangesAsync();


                await _purchaseOrderService.RecalculateParentAsync(entity.PurchaseOrderId);
            }
        }



    }
}
