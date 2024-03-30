using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class CompanyConfiguration : _BaseConfiguration<Company>
    {
        public override void Configure(EntityTypeBuilder<Company> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Currency).IsRequired().HasMaxLength(50);
            builder.Property(c => c.TimeZone).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(255);
            builder.Property(c => c.Street).HasMaxLength(100);
            builder.Property(c => c.City).HasMaxLength(100);
            builder.Property(c => c.State).HasMaxLength(100);
            builder.Property(c => c.ZipCode).HasMaxLength(100);
            builder.Property(c => c.Country).HasMaxLength(100);
            builder.Property(c => c.PhoneNumber).HasMaxLength(20);
            builder.Property(c => c.FaxNumber).HasMaxLength(20);
            builder.Property(c => c.EmailAddress).HasMaxLength(100);
            builder.Property(c => c.Website).HasMaxLength(100);
            builder.Property(c => c.WhatsApp).HasMaxLength(20);
            builder.Property(c => c.LinkedIn).HasMaxLength(100);
            builder.Property(c => c.Facebook).HasMaxLength(100);
            builder.Property(c => c.Instagram).HasMaxLength(100);
            builder.Property(c => c.TwitterX).HasMaxLength(100);
            builder.Property(c => c.TikTok).HasMaxLength(100);
        }
    }
}
