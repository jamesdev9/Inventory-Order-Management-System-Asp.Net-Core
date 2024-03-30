using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.NormalizedUserName).HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.NormalizedEmail).HasMaxLength(100);
            builder.Property(c => c.FullName).HasMaxLength(100);
            builder.Property(c => c.JobTitle).HasMaxLength(100);
            builder.Property(c => c.Address).HasMaxLength(100);
            builder.Property(c => c.City).HasMaxLength(100);
            builder.Property(c => c.State).HasMaxLength(100);
            builder.Property(c => c.Country).HasMaxLength(100);
            builder.Property(c => c.ZipCode).HasMaxLength(100);
            builder.Property(c => c.PhoneNumber).HasMaxLength(50);
            builder.Property(c => c.Avatar).HasMaxLength(50);
            builder.Property(c => c.SelectedCompanyId).HasMaxLength(50).IsRequired();

            builder.Property(c => c.IsDefaultAdmin).HasDefaultValue(false);
            builder.Property(c => c.IsOnline).HasDefaultValue(false);
            builder.Property(c => c.IsNotDeleted).HasDefaultValue(true);
        }
    }

}
