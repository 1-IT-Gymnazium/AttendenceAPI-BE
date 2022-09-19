using Microsoft.AspNetCore.Mvc;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
using AttendenceApi.Services;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Utils;

namespace AttendenceApi.Controllers
{
    [Authorize]
    [ApiController]
    public class AbsenceController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly Claim _userClaim;
        private readonly Claim _adminClaim;
        public AbsenceController(IUserService userService, AppDbContext context, ILogger<AuthController> logger, IHttpContextAccessor contextAccessor, UserManager<User> user, SignInManager<User> signInManager)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _userManager = user;
            _signInManager = signInManager;
            _userClaim = new Claim("CLAIM_USER", Claims.USER);
            _adminClaim = new Claim(Claims.SUPERUSER, Claims.SUPERUSER);

        }

        [HttpGet("GetUserAbsence")]
        public async Task<ActionResult<List<Absence>>> GetUserAbsence()
        {
            var user = GetUserPrincipalFromContext();
            return _context.Absences.Where(s => user.GetUserId() == s.UserId).ToList();
        }
        public async Task<ActionResult<List<Absence>>> GetSpecificUserAbsence(string userName)
        {
            var user = _context.Users.SingleOrDefault(s => s.UserName == userName);
            return _context.Absences.Where(s => user.Id == s.UserId).ToList();
        }
        [HttpPost("Absence/Write")]
        public async Task<ActionResult> AbsenceZapis([FromBody] string Isic)
        {
            //gets users isic based on isic scanned with RFID
            var isic = _context.Isics
                .SingleOrDefault(s => s.IsicId == Isic);

            //gets user based on user id stored in isic
            var User = _context.Users
                .SingleOrDefault(User => User.Id == isic.UserId);

            //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
            var schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == User.ClassId && s.Date == DateTime.Today);
            if (schedule == null)
            {
                var sched = _context.Schedules.SingleOrDefault(Day => Day.ClassId == User.ClassId && Day.Day == DateTime.Today.ToString());
                //schedule = new AlteredSchedule { Class = sched.Class}
            }


            // Saves if student is already in school or not
            //var inSchool = new InSchool { UserId = User.Id };



            
            if (((int)DateTime.Now.TimeOfDay.TotalMinutes) < 515 && User.InSchool == false) // if user isnt in school and isnt late based on start time of the first hour in students schedule
            {
                //Saves student into in school DB (doesnt write him any absence)
                User.InSchool == true;
                _context.Update(User);
                _context.SaveChanges();
                return Ok();
            }
            if (((int)DateTime.Now.TimeOfDay.TotalMinutes) > 515 && User.InSchool == false) // if user isnt in school and is late based on start time of the first hour
            {
               
                var content = new Absence { UserId = isic.UserId, TimeOfArrival = DateTime.Now, Excused = false }; //inicializes new absence for the student

                _context.Absences.Add(content); // adds absence to DB

                User.InSchool = true; //sets user as in school

                _context.Users.Update(User); //updates user in DB
               

                _context.SaveChanges();

                return Ok();

            }



            private ClaimsPrincipal GetUserPrincipalFromContext()
            {
                var user = _signInManager.Context.User;

                _ = _contextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
                return _contextAccessor.HttpContext.User;
            }

        }
    }
}
