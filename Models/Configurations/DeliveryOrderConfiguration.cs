using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class DeliveryOrderConfiguration : _BaseConfiguration<DeliveryOrder>
    {
        public override void Configure(EntityTypeBuilder<DeliveryOrder> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.SalesOrderId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);
        }
    }
}
