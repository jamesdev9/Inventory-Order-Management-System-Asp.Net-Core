using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class InventoryTransactionConfiguration : _BaseConfiguration<InventoryTransaction>
    {
        public override void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.ModuleId).IsRequired();
            builder.Property(c => c.ModuleName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ModuleCode).IsRequired().HasMaxLength(10);
            builder.Property(c => c.ModuleNumber).IsRequired().HasMaxLength(100);
            builder.Property(c => c.MovementDate).IsRequired();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);

            builder.HasOne(c => c.Warehouse)
                .WithMany()
                .HasForeignKey(c => c.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.WarehouseFrom)
                .WithMany()
                .HasForeignKey(c => c.WarehouseFromId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.WarehouseTo)
                .WithMany()
                .HasForeignKey(c => c.WarehouseToId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
