using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class TransferOutConfiguration : _BaseConfiguration<TransferOut>
    {
        public override void Configure(EntityTypeBuilder<TransferOut> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.WarehouseFromId).IsRequired();
            builder.Property(c => c.WarehouseToId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);

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
