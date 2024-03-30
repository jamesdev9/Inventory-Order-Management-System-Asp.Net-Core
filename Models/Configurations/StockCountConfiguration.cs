using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class StockCountConfiguration : _BaseConfiguration<StockCount>
    {
        public override void Configure(EntityTypeBuilder<StockCount> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.WarehouseId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);
        }
    }
}
