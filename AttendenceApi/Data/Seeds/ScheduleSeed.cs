using AttendenceApi.Controllers;

namespace AttendenceApi.Data.Seeds
{
    public class ScheduleSeed
    {
        public static async Task CreateSchedule(AppDbContext dbContext)
        {
            Schedule[] schedule =
            {
                new Schedule {
                ClassId = AuthController.GuidFromString("4.T"),
                Date = "2022-04-20",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Monday"
                },
                                new Schedule {
                ClassId = AuthController.GuidFromString("4.T"),
                Date = "2022-04-21",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Tuesday"
                },
                                                new Schedule {
                ClassId = AuthController.GuidFromString("4.T"),
                Date = "2022-04-22",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Wednesday"
                },
                                                                new Schedule {
                ClassId = AuthController.GuidFromString("4.T"),
                Date = "2022-04-23",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Thursday"
                },
                                                                                new Schedule {
                ClassId = AuthController.GuidFromString("4.T"),
                Date = "2022-04-24",
                EndTimeOfLessonsInMinutes = 810,
                Day = "Friday"
                },

            };
           var isindb = dbContext.Schedules.FirstOrDefault(s => s.ClassId == schedule[0].ClassId && s.Date == "2022-04-20");
            if(isindb == null)
            {
                for (int i = 0; i < schedule.Length; i++)
                {
                    await dbContext.AddAsync(schedule[i]);
                }
                
                await dbContext.SaveChangesAsync();

            }

        }
    }
}
