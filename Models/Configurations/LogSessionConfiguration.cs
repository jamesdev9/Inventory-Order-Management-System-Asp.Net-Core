using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Indotalent.Models.Configurations
{
    public class LogSessionConfiguration : _BaseConfiguration<LogSession>
    {
        public override void Configure(EntityTypeBuilder<LogSession> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.UserId).HasMaxLength(50);
            builder.Property(c => c.IPAddress).HasMaxLength(20);
            builder.Property(c => c.Action).HasMaxLength(50);
        }


    }
}
