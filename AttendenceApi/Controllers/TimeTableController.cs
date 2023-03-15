using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AttendenceApi.Controllers
{
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


        [Authorize]
        [HttpGet("Get/Timetable")]
        public IActionResult GetTimeTable()
        {
           
            var User = _context.Users.FirstOrDefault(s => s.UserName == Request.HttpContext.User.Identity.Name);

            //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
            var schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == User.ClassId && s.Date == DateTime.Today);
           
            
            List<Lesson> hours = null;
            if (schedule == null)
            {
                var uzfaktnevim = _context.Schedules;
                var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
                var sched = _context.Schedules.First(Day => Day.ClassId == User.ClassId && Day.Day == currentDay);
                hours = _context.Lessons.Where(s => s.ScheduleId == sched.Id).OrderBy(l => l.LessonIndex).ToList();
                return Ok(sched);
            }
            else
            {
                hours = _context.Lessons.Where(s => s.ScheduleId == schedule.Id).OrderBy(l => l.LessonIndex).ToList();
                return Ok(schedule);
            }
            
        }

        [HttpPost("Get/SpecificTimeTable")]
        public IActionResult GetSpecificTimeTable(string Date, string ClassId)
        {

            var idclass = GuidFromString(ClassId.ToUpper());
            var Schedule = _context.Schedules.Single(s => s.ClassId == idclass && Date == s.Date);
            var lessons = _context.Lessons.Where(s => s.ScheduleId == Schedule.Id).ToList();
            var lsnvm = new List<LessonVm>();
            var date = new DateTime();
            
            for (int i = 0; i < lessons.Count; i++)
            {
                lsnvm.Add(new LessonVm { Name = lessons[i].Name, LessonIndex = lessons[i].LessonIndex, ScheduleId = lessons[i].ScheduleId, Teacher = lessons[i].Teacher });
            }
            var output = new List<ScheduleVM>();
            output.Add(new ScheduleVM { ClassId = Schedule.Id, Lessons = lsnvm, Date = Schedule.Date, Day = Schedule.Day, EndTimeOfLessonsInMinutes = Schedule.EndTimeOfLessonsInMinutes });
            return Ok(output); 
        }
        [AllowAnonymous]
        [HttpPost("Create/x x")]
        public IActionResult CreateTimeTable([FromBody] List<CreateScheduleVM> model)
        {
          
            
            
            
            for (int i = 0; i < model.Count; i++)
            {
                var id = new Guid();
                var sche = new AlteredSchedule();
                var lessons = new List<Lesson>();
                    sche.EndTimeOfLessonsInMinutes = model[i].EndTimeOfLessonsInMinutes;
                sche.ClassId = GuidFromString(model[i].ClassId);
                sche.Date = DateTime.Parse(model[i].Date);
                sche.StartOfLessonsInMinutes = model[i].StartTimeOfLessonsInMinutes;
                sche.Id = id;
               
                for (int s = 0; s < model[i].Lessons.Count; s++)
                {
                    //var teacher = await _context.Users.SingleOrDefaultAsync(x => x.UserName == model[i].Lessons[s].Teacher);
                    //lessons.Add(new Lesson {EndTimeInMinutes = model[i].Lessons[s].EndTimeInMinutes, LessonIndex = model[i].Lessons[s].LessonIndex, Name = model[i].Lessons[s].Name, TeacherId = teacher.Id, ScheduleId = id, StartTimeInMinutes = model[i].Lessons[s].StartTimeInMinutes });

                }
                _context.AlteredSchedules.Add(sche);
                foreach (var lesson in lessons)
                {
                    _context.Lessons.Add(lesson);
                }
                _context.SaveChangesAsync();
            }
            return Ok();

        }

        public IActionResult GetClassFromSchedule()
        {
            return Ok();
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
