using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.Utils;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AttendenceApi.Controllers
{
    [Authorize]
    public class TimeTableController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public TimeTableController(IUserService userService, AppDbContext context, ILogger<AuthController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }


        
        [HttpGet("Get/Timetable")] 
        public async Task<IActionResult> GetTimeTable()
        {

            var user = _context.Users.FirstOrDefault(s => s.UserName == Request.HttpContext.User.Identity.Name);
            if (user == null) // if user isnt found
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} wasnt found");
                return NotFound();
            }

            var currentUserSubjects = _context.StudentSubjects.Where(s => s.StudentId == user.Id);

            //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
            var schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == user.ClassId && s.Date == DateTime.Today);


            List<Lesson> hours = null;
            if (schedule == null)
            {        

                // Sort the list based on the index of each day in 'dayOrder'
                // Define the correct order of days starting from Monday
                List<string>dayOrder = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

                
             

                var uzfaktnevim = _context.Schedules;
                var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
                var fullschedule = new List<ScheduleVM2>();
                var sched = _context.Schedules.Where(Day => Day.ClassId == user.ClassId).ToList();
                sched = sched.OrderBy(day => dayOrder.IndexOf(day.Day)).ToList();
               
                foreach (var item in sched)
                {
                    hours = _context.Lessons.Where(s => s.ScheduleId == item.Id).OrderBy(l => l.LessonIndex).ToList();

                    fullschedule.Add(new ScheduleVM2 { schedule = item, lessons = hours });
                }
                fullschedule.OrderBy(s=> s.schedule.Day).ToList();

                return Ok(fullschedule);
            }
            else
            {
                var today = DateTime.Now.Date; // This can be any date.

              
                var alldays = new List<DateTime>(); 
                var fullschedule = new List<ScheduleVM2>();
                alldays = AbsenceController.GetWeekDays(today).ToList(); // gets all the days of the week
                foreach (var day in alldays) // gets all the schedules for the week into VM and assigns lessons to each one of them
                {
                    var dayschedule = _context.AlteredSchedules.FirstOrDefaultAsync(s => s.Date == day); 
                    hours = _context.Lessons.Where(s => s.AlteredScheduleId == dayschedule.Result.Id).OrderBy(l => l.LessonIndex).ToList();

                    fullschedule.Add(new ScheduleVM2 { alteredSchedule = dayschedule.Result, lessons = hours });
                }

 
               
                return Ok(fullschedule);
            }

        }

       
        [HttpPost("Create/Timetable")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> CreateTimeTable([FromBody] List<CreateScheduleVM> model)
        {
            // Log the start of the timetable creation process
            _logger.LogInformation("Starting to create timetable");

            for (int i = 0; i < model.Count; i++)
            {
                // Generate a new GUID for the schedule
                var id = Guid.NewGuid();
                _logger.LogDebug("Generated new GUID for the schedule: " + id);

                var sche = new AlteredSchedule
                {
                    EndTimeOfLessonsInMinutes = model[i].EndTimeOfLessonsInMinutes,
                    ClassId = GuidFromString(model[i].ClassId),
                    Date = DateTime.Parse(model[i].Date),
                    StartOfLessonsInMinutes = model[i].StartTimeOfLessonsInMinutes,
                    Id = id
                };

                var lessons = new List<Lesson>();

                for (int s = 0; s < model[i].Lessons.Count; s++)
                {
                    // Retrieve teacher details from the database
                    
                    var teacher = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model[i].Lessons[s].Teacher);
                    if (teacher == null)
                    {
                        _logger.LogWarning("Teacher not found: " + model[i].Lessons[s].Teacher);
                        continue;
                    }
                    var lessonId = Guid.NewGuid();
                    lessons.Add(new Lesson
                    {
                        Id = lessonId,
                        Room = model[i].Lessons[s].Room,
                        EndTimeInMinutes = model[i].Lessons[s].EndTimeInMinutes,
                        LessonIndex = model[i].Lessons[s].LessonIndex,
                        Name = model[i].Lessons[s].Name,
                        TeacherId = teacher.Id,
                        AlteredScheduleId = sche.Id,
                        StartTimeInMinutes = model[i].Lessons[s].StartTimeInMinutes
                    });
                }

                _context.AlteredSchedules.Add(sche);
                foreach (var lesson in lessons)
                {
                    _context.Lessons.Add(lesson);
                }

                // Log the addition of schedule and lessons to the context
                _logger.LogDebug($"Added schedule and lessons to the database context for processing");

                // Save changes to the database
                await _context.SaveChangesAsync();
                _logger.LogInformation("Changes saved to the database for schedule "+ id);
            }

            // Log the successful creation of the timetable
            _logger.LogInformation("Timetable creation completed successfully");

            return Ok("Timetable succesfully created");

        }


        [HttpPost("Delete/Timetable")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> DeleteTimetable([FromBody] TimetableVM content) // removes Timetable from DB using class and date, only works for altered timetables not for base timetable
        {
            var classid = GuidFromString(content.Class);
            var date = DateTime.Parse(content.Date).Date;
            var schedule = await _context.AlteredSchedules.FirstOrDefaultAsync(s => s.ClassId == classid && s.Date == date);
            if (schedule == null)
            {
                _logger.LogError("Timetable for deletion not found");
                return BadRequest("Timetable not found");
            }
            var lessons = await _context.Lessons.Where(s => s.AlteredScheduleId == schedule.Id).ToListAsync();
            foreach (var lesson in lessons)
            {
                _context.Remove(lesson);
            }
            _context.Remove(schedule);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Timetable removed");
            return Ok("Timetable removed");
        }


        [HttpPost("Create/EverydayTimetable")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> CreateEverydayTimetable([FromBody] List<EverydayScheduleVM> model)
        {
            if (model == null)
            {
                return BadRequest("No data sent");
            }

            var oldSchedules = await _context.Schedules.Where(s => s.ClassId == s.ClassId).ToListAsync();

            for (int i = 0; i < model.Count; i++)
            {
                
                var old = oldSchedules.FirstOrDefault(s=> s.Day == model[i].Day);
                var lessons = await _context.Lessons.Where(s => s.ScheduleId == old.Id).ToListAsync();
                foreach (var lesson in lessons)
                {
                    _context.Remove(lesson);

                }
                await _context.SaveChangesAsync();
                lessons = new List<Lesson>();
                
                old.EndTimeOfLessonsInMinutes = model[i].EndTimeOfLessonsInMinutes;
                old.ClassId = GuidFromString(model[i].ClassId);
               
                old.StartTimeOfLessonsInMinutes= model[i].StartTimeOfLessonsInMinutes;
         

                for (int s = 0; s < model[i].Lessons.Count; s++)
                {
                    var teacher = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model[i].Lessons[s].Teacher);
                    if (teacher == null)
                    {
                        return BadRequest($"Teacher {model[i].Lessons[s].Teacher} not found");
                    }
                    

                   
                    lessons.Add(new Lesson { EndTimeInMinutes = model[i].Lessons[s].EndTimeInMinutes, LessonIndex = model[i].Lessons[s].LessonIndex, Name = model[i].Lessons[s].Name, TeacherId = teacher.Id, ScheduleId = old.Id, StartTimeInMinutes = model[i].Lessons[s].StartTimeInMinutes, Parity = model[i].Lessons[s].Parity, Room = model[i].Lessons[s].Room });

                }
                
                foreach (var lesson in lessons)
                {
                    _context.Lessons.Add(lesson);
                }
                await _context.SaveChangesAsync();
            }
            return Ok("Timetable Added");

        }
      




        private static Guid GuidFromString(string input)
        {

            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }


    }
}
