using AttendenceApi.Controllers;

namespace AttendenceApi.Data.Seeds
{
    public class ScheduleSeed
    {
        public static async Task CreateSchedule(AppDbContext dbContext)
        {
            var schedule = new Schedule
            {
                ClassId = AuthController.GuidFromString("1.A"),
                Date = "2022-04-20",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Monday",
                

            };
           var isindb = dbContext.Schedules.FirstOrDefault(s => s.ClassId == schedule.ClassId && s.Date == "2022-04-20");
            if(isindb == null)
            {
                await dbContext.Schedules.AddAsync(schedule);
                await dbContext.SaveChangesAsync();

            }

        }
    }
}
