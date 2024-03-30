using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class CustomerContactConfiguration : _BaseConfiguration<CustomerContact>
    {
        public override void Configure(EntityTypeBuilder<CustomerContact> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.CustomerId).IsRequired();
            builder.Property(c => c.Number).HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);
            builder.Property(c => c.PhoneNumber).HasMaxLength(20);
            builder.Property(c => c.EmailAddress).HasMaxLength(100);
        }
    }
}
