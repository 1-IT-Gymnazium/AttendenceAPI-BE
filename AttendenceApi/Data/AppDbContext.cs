using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AttendenceApi.Data
{
    public class AppDbContext : IdentityDbContext<User,Role,int>
    {
        public DbSet<Absence> Absences { get; set; }
        public DbSet<AlteredSchedule> AlteredSchedules { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Isic> Isics { get; set; }
        public DbSet<Schedule> Schedules { get; set; }



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
