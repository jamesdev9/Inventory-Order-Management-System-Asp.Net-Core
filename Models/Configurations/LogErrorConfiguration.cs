using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class LogErrorConfiguration : _BaseConfiguration<LogError>
    {
        public override void Configure(EntityTypeBuilder<LogError> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.ExceptionMessage).HasMaxLength(5000);
            builder.Property(c => c.StackTrace).HasMaxLength(5000);
            builder.Property(c => c.AdditionalInfo).HasMaxLength(5000);
        }


    }
}
