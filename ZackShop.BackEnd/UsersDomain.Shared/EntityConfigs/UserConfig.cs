using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersDomain.Shared.Entities;

namespace UsersDomain.Shared.EntityConfigs
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(e=>e.Email).IsUnique();
        }
    }
}
