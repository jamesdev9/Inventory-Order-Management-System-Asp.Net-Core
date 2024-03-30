using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class LogAnalyticConfiguration : _BaseConfiguration<LogAnalytic>
    {
        public override void Configure(EntityTypeBuilder<LogAnalytic> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.UserId).HasMaxLength(50);
            builder.Property(c => c.IPAddress).HasMaxLength(20);
            builder.Property(c => c.Url).HasMaxLength(100);
            builder.Property(c => c.Device).HasMaxLength(50);
            builder.Property(c => c.GeographicLocation).HasMaxLength(100);
            builder.Property(c => c.Browser).HasMaxLength(50);
        }


    }
}
