using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.Seeds
{
    public class AbsenceSeed
    {
        public static async Task CreateAbsence(UserManager<User> userManager, AppDbContext dbcontext)
        {
            var user = userManager.FindByEmailAsync("Example@Example.com").Result;
            var absence = new Absence
            {
                Date = DateTime.UtcNow,
                Excused = false,
                TimeOfArrival = DateTime.UtcNow,
                UserId = user.Id
                
                
            };
            var abs = dbcontext.Absences.Where(a => a.UserId == absence.UserId).ToList();
            if (abs.Count == 0)
            {
                dbcontext.Absences.Add(absence);
               await  dbcontext.SaveChangesAsync();

            }

        }
    }
}
