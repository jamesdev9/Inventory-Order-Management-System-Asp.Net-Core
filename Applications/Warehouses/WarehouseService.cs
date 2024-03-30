using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Contracts;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Applications.Warehouses
{
    public class WarehouseService : Repository<Warehouse>
    {
        public WarehouseService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }



        public override async Task UpdateAsync(Warehouse? entity)
        {
            if (entity != null)
            {

                if (entity.SystemWarehouse)
                {
                    _context.ChangeTracker.Clear();
                    throw new Exception("Updating system warehouse is prohibited");
                }

                if (entity is IHasAudit auditEntity && !string.IsNullOrEmpty(_userId))
                {
                    auditEntity.UpdatedByUserId = _userId;
                }
                if (entity is IHasAudit auditedEntity)
                {
                    auditedEntity.UpdatedAtUtc = DateTime.Now;
                }

                _context.Set<Warehouse>().Update(entity);
                await _context.SaveChangesAsync();

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

            var entity = await _context.Set<Warehouse>()
                .FirstOrDefaultAsync(x => x.Id == id);


            if (entity != null)
            {
                if (entity.SystemWarehouse)
                {
                    throw new Exception("Deleting system warehouse is prohibited");
                }

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
                    _context.Set<Warehouse>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }

        public override async Task DeleteByRowGuidAsync(Guid? rowGuid)
        {
            if (!rowGuid.HasValue)
            {
                throw new Exception("Unable to process, row guid is null");
            }

            var entity = await _context.Set<Warehouse>()
                .FirstOrDefaultAsync(x => x.RowGuid == rowGuid);

            if (entity != null)
            {
                if (entity.SystemWarehouse)
                {
                    throw new Exception("Deleting system warehouse is prohibited");
                }

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
                    _context.Set<Warehouse>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }

        public Warehouse? GetCustomerWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "Customer").FirstOrDefault();
        }

        public Warehouse? GetVendorWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "Vendor").FirstOrDefault();
        }

        public Warehouse? GetTransferWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "Transfer").FirstOrDefault();
        }

        public Warehouse? GetAdjustmentWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "Adjustment").FirstOrDefault();
        }

        public Warehouse? GetStockCountWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "StockCount").FirstOrDefault();
        }

        public Warehouse? GetScrappingWarehouse()
        {
            return _context.Set<Warehouse>().Where(x => x.Name == "Scrapping").FirstOrDefault();
        }

    }
}
