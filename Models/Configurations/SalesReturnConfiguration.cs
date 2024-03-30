using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class SalesReturnConfiguration : _BaseConfiguration<SalesReturn>
    {
        public override void Configure(EntityTypeBuilder<SalesReturn> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.DeliveryOrderId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);
        }
    }
}
