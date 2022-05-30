using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetSpecificTimeTable(string Date, Guid ClassId)
        {
            var Schedule = _context.Schedules.SingleOrDefault(s => s.ClassId == ClassId && Date == s.Date);
            return Ok(Schedule);
        }

    }
}
