using Microsoft.AspNetCore.Mvc;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        public async Task<ActionResult<List<AbsenceIndexVM>>> GetUserAbsence()
        {
            var claimsPrincipal = GetUserPrincipalFromContext();
            var user = _context.Users.FirstOrDefault(s => claimsPrincipal.GetUserId() == s.Id);

            var absenceList = _context.Absences.Where(s => user.Id == s.UserId).ToList();
            var output = new List<AbsenceIndexVM>();    

            foreach (Absence absence in absenceList)
            {
                List<Lesson> hours = null;
                int? AbsenceArrivalInMinutes = TimeOfDayToMinutes(absence.TimeOfArrival);
                int? AbsenceExitinMinutes = TimeOfDayToMinutes(absence.TimeOfExit);
                List<int> MissedHoursIndex = new List<int>();
                AlteredSchedule? schedule = new AlteredSchedule();
                //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
                try
                {
                    schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == user.ClassId && s.Date.Date == absence.TimeOfArrival.Value.Date);

                }
                catch
                {
                    schedule = null;
                }


                if (schedule == null)
                {
                    
                    var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
                    var sched = _context.Schedules.First(Day => Day.ClassId == user.ClassId && Day.Day == currentDay);
                    hours = _context.Lessons.Where(s => s.ScheduleId == sched.Id).OrderBy(l => l.LessonIndex).ToList();
                }
                else
                {
                    hours = _context.Lessons.Where(s => s.ScheduleId == schedule.Id).OrderBy(l => l.LessonIndex).ToList();

                }

                for (int i = 0; i < hours.Count; i++) // works but not tested a lot
                {




                     if (AbsenceArrivalInMinutes == null && AbsenceExitinMinutes <= hours[i].StartTimeInMinutes || AbsenceExitinMinutes <= hours[i].EndTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex); //user absent
                        continue;
                    }
                    else if (AbsenceExitinMinutes == null && AbsenceArrivalInMinutes >= hours[i].EndTimeInMinutes || AbsenceArrivalInMinutes >= hours[i].StartTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);//user absent
                        continue;
                    }
                    else if (AbsenceArrivalInMinutes >= hours[i].EndTimeInMinutes || AbsenceExitinMinutes <= hours[i].StartTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);//user absent
                        continue;

                    }
                    else
                    {
                        //user was present
                    }



                }
                output.Add(new AbsenceIndexVM { Absence = absence, LessonIndexes = MissedHoursIndex });
            }
            return output;
        }


        [Authorize(Policy = Policies.SUPERADMIN)]
        [HttpPost("GetSpecificUser")]
        public async Task<ActionResult<List<AbsenceIndexVM>>> GetSpecificUserAbsence(string userName)
        {
            
            var user = _context.Users.FirstOrDefault(s => s.NormalizedUserName == userName);
            if (user == null)
            {
                return NotFound("User Not Found"); 
            }

            var absenceList = _context.Absences.Where(s => user.Id == s.UserId).ToList();
            var output = new List<AbsenceIndexVM>();

            foreach (Absence absence in absenceList)
            {
                List<Lesson> hours = null;
                int? AbsenceArrivalInMinutes = TimeOfDayToMinutes(absence.TimeOfArrival);
                int? AbsenceExitinMinutes = TimeOfDayToMinutes(absence.TimeOfExit);
                List<int> MissedHoursIndex = new List<int>();
                AlteredSchedule? schedule = new AlteredSchedule();
                //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
                try
                {
                    schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == user.ClassId && s.Date.Date == absence.TimeOfArrival.Value.Date);

                }
                catch
                {
                    schedule = null;
                }


                if (schedule == null)
                {

                    var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
                    var sched = _context.Schedules.First(Day => Day.ClassId == user.ClassId && Day.Day == currentDay);
                    hours = _context.Lessons.Where(s => s.ScheduleId == sched.Id).OrderBy(l => l.LessonIndex).ToList();
                }
                else
                {
                    hours = _context.Lessons.Where(s => s.ScheduleId == schedule.Id).OrderBy(l => l.LessonIndex).ToList();

                }

                for (int i = 0; i < hours.Count; i++) // works but not tested a lot
                {




                    if (AbsenceArrivalInMinutes == null && AbsenceExitinMinutes <= hours[i].StartTimeInMinutes || AbsenceExitinMinutes <= hours[i].EndTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex); //user absent
                        continue;
                    }
                    else if (AbsenceExitinMinutes == null && AbsenceArrivalInMinutes >= hours[i].EndTimeInMinutes || AbsenceArrivalInMinutes >= hours[i].StartTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);//user absent
                        continue;
                    }
                    else if (AbsenceArrivalInMinutes >= hours[i].EndTimeInMinutes || AbsenceExitinMinutes <= hours[i].StartTimeInMinutes)
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);//user absent
                        continue;

                    }
                    else
                    {
                        //user was present
                    }



                }
                output.Add(new AbsenceIndexVM { Absence = absence, LessonIndexes = MissedHoursIndex });
            }
            return Ok(output);
        }

        [Authorize(Policy = Policies.SUPERADMIN)]
        [HttpGet("SetAllUsersToNotInSchool")]
        public async Task<IActionResult> SetUsersToNotInSchool()
        {

            var users = _context.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].InSchool = false;
                _context.Update(users[i]);
            }
           
            _context.SaveChanges();

            return Ok("Users set to not in school");
        }


        
        [HttpPost("Absence/Write")]
        [AllowAnonymous]
        public async Task<ActionResult> AbsenceZapis([FromBody] string Isic)
        {
            //gets users isic based on isic scanned with RFID
            var isic = _context.Isics
                .SingleOrDefault(s => s.IsicId == Isic);

            //gets user based on user id stored in isic
            var User = _context.Users
                .SingleOrDefault(User => User.Id == isic.UserId);
            List<Lesson> hours = null;

            //gets Altered schedule for current day, if there isnt a different schedule it finds casual everyday schedule
            var schedule = _context.AlteredSchedules.SingleOrDefault(s => s.ClassId == User.ClassId && s.Date == DateTime.Today);
            
            if (schedule == null)
            {
                var uzfaktnevim = _context.Schedules;
                var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
                var sched = _context.Schedules.First(Day => Day.ClassId == User.ClassId && Day.Day == currentDay);
                hours = _context.Lessons.Where(s => s.ScheduleId == sched.Id).OrderBy(l => l.LessonIndex).ToList();
            }
            else
            {
                 hours = _context.Lessons.Where(s => s.ScheduleId == schedule.Id).OrderBy(l => l.LessonIndex).ToList();
               
            }




            if (User.InSchool)
            {
                return Ok("AlreadyInSchool");
            }
            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) < (hours.First().StartTimeInMinutes + 5) && User.InSchool == false) // if user isnt in school and isnt late based on start time of the first hour in students schedule
            {
                //Saves student into in school DB (doesnt write him any absence)
                User.InSchool = true;
                _context.Update(User);
                _context.SaveChanges();
                return Ok();
            }
            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) > (hours.First().StartTimeInMinutes + 5) && User.InSchool == false) // if user isnt in school and is late based on start time of the first hour
            {

                var content = new Absence { UserId = isic.UserId, TimeOfArrival = DateTime.UtcNow.AddHours(2), Excused = false, Date = DateTime.UtcNow.AddHours(2).Date }; //inicializes new absence for the student

                _context.Absences.Add(content); // adds absence to DB

                User.InSchool = true; //sets user as in school

                _context.Users.Update(User); //updates user in DB


                _context.SaveChanges();

                return Ok();

            }



            return BadRequest();

        }
        
        [HttpPost("excuse")]
        public async Task<IActionResult> ExcuseAbsence([FromBody] List<AbsenceExcuseVM> content, int pin) //Endpoint for excusing, takes list of dates and reasons and 1 pin code 
        {
            
            var user = _context.Users.SingleOrDefault(s => s.Id == GetUserPrincipalFromContext().GetUserId()); // gets user from DB
            if (user == null) // if user isnt found
            {
                return NotFound();
            }
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, pin);

            if (user.PinHash == pin.ToString()) // if pin that the user sent matches the pin in database 
            {
                for (int i = 0; i < content.Count; i++) // for to iterate through all the sent absences
                {
                    Absence absence = _context.Absences.SingleOrDefault(s => s.UserId == user.Id && s.Date == content[i].Date && s.Excused == false); // finds absence in database based on user id and date of the absence
                    absence.Excused = true; //changes absences status to excused
                    absence.Reason = content[i].Reason; // writes reason of missing in database
                    _context.Update(absence); // updates the absence in database
                    
                   
                }
                _context.SaveChanges();
                return Ok("Succesfully excused");
            }
            else
            {
                return BadRequest("Bad Request");
            }
           
           


        }

        
        [HttpPost("/writeAbsence2")]
        [AllowAnonymous]
        public async Task<IActionResult> AbsenceWrite([FromBody] string Isic) //Used to write absences without all the fancy fuzzy stuff
        {
            var isic = _context.Isics
                .SingleOrDefault(s => s.IsicId == Isic);

            //gets user based on user id stored in isic
            var User = _context.Users
                .SingleOrDefault(User => User.Id == isic.UserId);
            if (User.InSchool == false)
            {
                var content = new Absence { UserId = isic.UserId, TimeOfArrival = DateTime.UtcNow.AddHours(2), Excused = false, Date = DateTime.UtcNow.AddHours(2).Date,Reason = "Came" };
                _context.Absences.Add(content);
                User.InSchool = true;
                _context.SaveChangesAsync();

                return Ok("Welcome");
            }
            if (User.InSchool == true)
            {
                var content = new Absence { UserId = isic.UserId, TimeOfArrival = DateTime.UtcNow.AddHours(2), Excused = false, Date = DateTime.UtcNow.AddHours(2).Date, Reason = "Left" };
                _context.Absences.Add(content);
                User.InSchool= false;
                _context.SaveChangesAsync();
                return Ok("Goodbye");
            }
            return BadRequest();


        }

        [HttpGet("SetIfUserInSchoolByTeacher")]
        [Authorize(Policy = Policies.SUPERADMIN)]
        public async Task<IActionResult> SetIfUserInSchoolByTeacher([FromBody] string username) //used for teachers if student forgot isic
        {
            
            var user = _context.Users.First(s => s.UserName == username);
            if (user == null)
            {
                _logger.LogError($"User {username} not found");
            }
            user.InSchool = !user.InSchool;
            _context.Update(user); 
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User {GetUserPrincipalFromContext().Identity.Name} changed InSchool of user {username}");
            return Ok("User In School changed to "+ user.InSchool);
        }




        [HttpGet("Absences/Get")]
        public async Task<IActionResult> GetAbsences()
        {
            var user = _context.Users.SingleOrDefault(s => s.Id == GetUserPrincipalFromContext().GetUserId());
            var absences = _context.Absences.Where(s => s.UserId == user.Id).OrderBy(s=> s.TimeOfArrival).ToList();
            var output = new List<GetAbsenceVM>();
            for (int i = 0; i < absences.Count; i += 2)
            {
                if (i + 2 > absences.Count)
                {
                    output.Add(new GetAbsenceVM { arrival = absences[absences.Count - 1].TimeOfArrival, exit = null });
                    break;
                }
                if (absences[i].Date == absences[i + 1].Date)
                {
                    output.Add(new GetAbsenceVM { arrival = absences[i].TimeOfArrival, exit = absences[i + 1].TimeOfArrival });
                }
                else
                {
                    output.Add(new GetAbsenceVM { arrival = absences[i].TimeOfArrival, exit = null });
                }
            }

            return Ok(output);
        }



        private ClaimsPrincipal GetUserPrincipalFromContext()
        {
            var user = _signInManager.Context.User;

            _ = _contextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
            return _contextAccessor.HttpContext.User;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, int input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(BitConverter.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static int? TimeOfDayToMinutes(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            return dateTime.Value.Hour * 60 + dateTime.Value.Minute;
        }
    }
}
