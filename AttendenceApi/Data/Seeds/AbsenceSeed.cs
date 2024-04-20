using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.Seeds
{
    public class AbsenceSeed
    {
        public static async Task CreateAbsence(UserManager<User> userManager, AppDbContext dbcontext)
        {
            var user = userManager.FindByNameAsync("User123").Result;


            Absence[] absence =
            {
                new Absence {                Date = DateTime.Parse("2024-01-08").ToUniversalTime(),
                Excused = false,
                TimeOfArrival = DateTime.Parse("2024-01-08 9:44:33.79807+00").ToUniversalTime(),
                UserId = user.Id},
                new Absence {                Date = DateTime.Parse("2023-01-09").ToUniversalTime(),
                Excused = true,
                TimeOfArrival = DateTime.Parse("2024-01-09 11:44:33.79807+00").ToUniversalTime(),
                UserId = user.Id},
                new Absence {                Date = DateTime.Parse("2023-01-10").ToUniversalTime(),
                Excused = false,
                TimeOfArrival = DateTime.Parse("2024-01-10 12:44:43.74236+00").ToUniversalTime(),
                UserId = user.Id},
                new Absence {                Date = DateTime.Parse("2024-01-11").ToUniversalTime(),
                Excused = false,
                TimeOfArrival = DateTime.Parse("2024-01-11 8:44:33.79807+00").ToUniversalTime(),
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
