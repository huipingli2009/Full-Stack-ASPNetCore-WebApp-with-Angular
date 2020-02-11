using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace org.cchmc.pho.identity.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
            new IdentityRole
            {
                Name = "Role1",
                NormalizedName = "ROLE1"
            },
            new IdentityRole
            {
                Name = "Role2",
                NormalizedName = "ROLE2"
            });
        }
    }
}
