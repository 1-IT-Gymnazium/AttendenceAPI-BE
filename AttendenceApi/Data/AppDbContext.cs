using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AttendenceApi.Data
{
    public class AppDbContext : IdentityDbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
           
            base.OnConfiguring(options);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var assemblyWithConfiguration = GetType().Assembly;
            builder.ApplyConfigurationsFromAssembly(assemblyWithConfiguration);
        }
    }
}
