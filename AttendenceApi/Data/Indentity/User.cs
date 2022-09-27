using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendenceApi.Data.Indentity
{
    public class User : IdentityUser<Guid>
    {
        public ICollection<IdentityUserClaim<Guid>> Claims { get; } = new HashSet<IdentityUserClaim<Guid>>();
        public Guid? ClassId { get; set; }
      public bool InSchool { get; set; }
        public string PinHash { get; set; } = string.Empty;

        public static class Entry
        {
            public const string ClaimTypeSuperUser = "CLAIM_SU";
        }
    }
 
    public class Configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasMany(x => x.Claims)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }


}
