using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AttendenceApi.Data
{
    public class AppDbContext : IdentityUserContext<User, Guid>
    {
        public DbSet<Absence> Absences { get; set; } = null!;
        public DbSet<AlteredSchedule> AlteredSchedules { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Isic> Isics { get; set; }  =null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        



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
