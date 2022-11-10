namespace AttendenceApi.Data.Seeds
{
    public class LessonSeed
    {
        public static async Task CreateLessons(AppDbContext dbContext)
        {
            var v = dbContext.Schedules.First().Id;
            var k = dbContext.Users.Where(s => s.UserName == "User123").First().Id;

            Lesson[] less  = { 
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 1,Name = "HUM", Room = "204",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  }, 
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 2,Name = "HUM", Room = "204",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 3,Name = "IFM", Room = "104",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 4,Name = "IFM", Room = "104",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 5,Name = "AJ", Room = "205",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = dbContext.Schedules.First().Id, LessonIndex = 6,Name = "ŠJ", Room = "205",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },

            };
            var isindb = dbContext.Lessons.FirstOrDefault(s => s.TeacherId == less[0].TeacherId);
           
            if (isindb == null)
            {
                for (int i = 0; i < less.Length; i++)
                {
                    await dbContext.AddAsync(less[i]);
                }
                
                await dbContext.SaveChangesAsync();

            }

        }
    }
}
