using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.Seeds
{
    public class AbsenceSeed
    {
        public static async Task CreateAbsence(UserManager<User> userManager, AppDbContext dbcontext)
        {
            var user = userManager.FindByEmailAsync("Example@Example.com").Result;


            Absence[] absence =
            {
                new Absence {                Date = DateTime.UtcNow.Date,
                Excused = false,
                TimeOfArrival = DateTime.UtcNow,
                UserId = user.Id},
                new Absence {                Date = DateTime.UtcNow.Date,
                Excused = true,
                TimeOfArrival = DateTime.UtcNow,
                UserId = user.Id},
                new Absence {                Date = DateTime.UtcNow.Date,
                Excused = false,
                TimeOfArrival = DateTime.UtcNow,
                UserId = user.Id},
                new Absence {                Date = DateTime.UtcNow.Date,
                Excused = false,
                TimeOfArrival = DateTime.UtcNow,
                UserId = user.Id}
            };
            var abs = dbcontext.Absences.Where(a => a.UserId == user.Id).ToList();
            if (abs.Count == 0)
            {
                for (int i = 0; i < absence.Length; i++)
                {
                    dbcontext.Absences.Add(absence[i]);
                }
                
               await  dbcontext.SaveChangesAsync();

            }

        }
    }
}
