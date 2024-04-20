using AttendenceApi.Controllers;

namespace AttendenceApi.Data.Seeds
{
    public class LessonSeed
    { 
        
        public static async Task CreateLessons(AppDbContext dbContext)
        {
            var v = dbContext.Schedules.Where(s=> s.ClassId == AuthController.GuidFromString("4.T")).ToArray();
            var k = dbContext.Users.Where(s => s.UserName == "User123").First().Id;
         
    


            Lesson[] less = {
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 1,Name = "HUM",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  }, 
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 2,Name = "HUM",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 3,Name = "IFM",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 4,Name = "IFM",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 5,Name = "AJ",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = v[0].Id, LessonIndex = 6,Name = "ŠJ",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 1,Name = "HUM",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 2,Name = "HUM",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 3,Name = "IFM",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 4,Name = "IFM",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 5,Name = "AJ",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = v[1].Id, LessonIndex = 6,Name = "ŠJ",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 1,Name = "HUM",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 2,Name = "HUM",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 3,Name = "IFM",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 4,Name = "IFM",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 5,Name = "AJ",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = v[2].Id, LessonIndex = 6,Name = "ŠJ",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 1,Name = "HUM",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 2,Name = "HUM",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 3,Name = "IFM",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 4,Name = "IFM",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 5,Name = "AJ",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = v[3].Id, LessonIndex = 6,Name = "ŠJ",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 1,Name = "HUM",StartTimeInMinutes = 510,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 555  },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 2,Name = "HUM",StartTimeInMinutes = 555,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 600 },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 3,Name = "IFM",StartTimeInMinutes = 620,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 665 },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 4,Name = "IFM",StartTimeInMinutes = 710,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 755 },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 5,Name = "AJ",StartTimeInMinutes = 760,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 805 },
                new Lesson {ScheduleId = v[4].Id, LessonIndex = 6,Name = "ŠJ",StartTimeInMinutes = 810,TeacherId = dbContext.Users.Where(s => s.UserName == "User123").First().Id,EndTimeInMinutes = 855 },
            }; 
            var isindb = dbContext.Lessons.FirstOrDefault(s => s.TeacherId == less[0].TeacherId);
           
            if (isindb == null)
            {
 
                foreach (var lesson in less)
                {
                    await dbContext.AddAsync(lesson);
                }
                
                await dbContext.SaveChangesAsync();

            }

        }
    }
}
