using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class VendorContactConfiguration : _BaseConfiguration<VendorContact>
    {
        public override void Configure(EntityTypeBuilder<VendorContact> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.VendorId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);
            builder.Property(c => c.PhoneNumber).HasMaxLength(20);
            builder.Property(c => c.EmailAddress).HasMaxLength(100);
        }
    }
}
