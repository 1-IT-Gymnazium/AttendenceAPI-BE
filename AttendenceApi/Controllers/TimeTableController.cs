using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var user = Request.HttpContext.User;
            return Ok();
        }

        [HttpPost("Get/SpecificTimeTable")]
        public IActionResult GetSpecificTimeTable(string Date, string ClassId)
        {

            var Schedule = _context.Schedules.Single(s => s.ClassId == GuidFromString(ClassId.ToUpper()) && Date == s.Date);
            var Lessons = _context.Lessons.Where(s => s.ScheduleId == Schedule.Id);
            var output = new List<Schedule<List<Lesson>>>();
            return Ok(Schedule); 
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
