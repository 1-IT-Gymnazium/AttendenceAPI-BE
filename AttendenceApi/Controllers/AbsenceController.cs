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
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

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
        public async Task<ActionResult<List<AbsenceIndexVM>>> GetUserAbsence() // this method is used for getting User Absence info including indexes of lessons missed
        {
            var claimsPrincipal = GetUserPrincipalFromContext();
            var user = _context.Users.FirstOrDefault(s => claimsPrincipal.GetUserId() == s.Id);
            // Gets all user absences

            var subjects = GetUserSubjects(user).Result;


            // Retrieve all absences for the user and log the count.
            var absenceList = _context.Absences.Where(s => user.Id == s.UserId).ToList();
            _logger.Log(LogLevel.Information, $"Found {absenceList.Count} absences for user {user.UserName}");

            if (absenceList.Count == 0)
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} has no absences");
                return Ok("User has no absence");
            }

            // Handle schedules and absences to determine missed lessons.
            var output = new List<AbsenceIndexVM>();
            foreach (Absence absence in absenceList)
            {
                List<int> MissedHoursIndex = new List<int>();
                //Get all the hours of the day
                var hours = GetLessonList(absence.Date.Value, user).Result;


                // Check missed hours based on user's absence details.
                for (int i = 0; i < hours.Count; i++)
                {
                    bool studentShouldBePresent = subjects.Any(sub => sub.Name == hours[i].Name);
                    int? AbsenceArrivalInMinutes = TimeOfDayToMinutes(absence.TimeOfArrival);
                    int? AbsenceExitinMinutes = TimeOfDayToMinutes(absence.TimeOfExit);
                    if (absenceCheck(AbsenceArrivalInMinutes, AbsenceExitinMinutes, hours[i], studentShouldBePresent))
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);
                        _logger.Log(LogLevel.Information, $"User {user.UserName} missed lesson index {hours[i].LessonIndex} on {absence.Date.Value.Date}");
                    }
                }
                output.Add(new AbsenceIndexVM { Absence = absence, LessonIndexes = MissedHoursIndex });
            }

            _logger.Log(LogLevel.Information, $"Absence processing complete for user {user.UserName}. Total processed absences: {output.Count}");
            return Ok(output);

        }



        [Authorize(Policy = Policies.TEACHER)]
        [HttpPost("GetSpecificUser")]
        public async Task<ActionResult<List<AbsenceIndexVM>>> GetSpecificUserAbsence(string userName) //kinda works but has to
        {

            var user = _context.Users.FirstOrDefault(s => s.UserName.ToUpper() == userName.ToUpper());
            if (user == null) // if user isnt found
            {
                _logger.Log(LogLevel.Information, $"Student {userName} wasnt found");
                return BadRequest("User not found");
            }


            // Gets all user absences


            var subjects = GetUserSubjects(user).Result;

            // Retrieve all absences for the user and log the count.
            var absenceList = _context.Absences.Where(s => user.Id == s.UserId).ToList();
            _logger.Log(LogLevel.Information, $"Found {absenceList.Count} absences for user {user.UserName}");

            if (absenceList.Count == 0)
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} has no absences");
                return Ok("User has no absence");
            }

            // Handle schedules and absences to determine missed lessons.
            var output = new List<AbsenceIndexVM>();
            foreach (Absence absence in absenceList)
            {
                List<int> MissedHoursIndex = new List<int>();

                //Get all the hours for the day
                var hours = GetLessonList(absence.Date.Value, user).Result;

                // Check missed hours based on user's absence details.
                for (int i = 0; i < hours.Count; i++)
                {
                    bool studentShouldBePresent = subjects.Any(sub => sub.Name == hours[i].Name);
                    int? AbsenceArrivalInMinutes = TimeOfDayToMinutes(absence.TimeOfArrival);
                    int? AbsenceExitinMinutes = TimeOfDayToMinutes(absence.TimeOfExit);
                    if (absenceCheck(AbsenceArrivalInMinutes, AbsenceExitinMinutes, hours[i], studentShouldBePresent))
                    {
                        MissedHoursIndex.Add(hours[i].LessonIndex);

                    }
                }
                output.Add(new AbsenceIndexVM { Absence = absence, LessonIndexes = MissedHoursIndex });
            }

            _logger.Log(LogLevel.Information, $"Absence processing complete for user {user.UserName}");
            return Ok(output);

        }

        [Authorize(Policy = Policies.TEACHER)]
        [HttpGet("SetAllUsersToNotInSchool")]
        public async Task<IActionResult> SetUsersToNotInSchool()
        {
            // Fetch all users from the database
            var users = _context.Users.ToList();

            // Set each user's InSchool status to false and update in database
            for (int i = 0; i < users.Count; i++)
            {
                users[i].InSchool = false;
                _context.Update(users[i]);
            }

            // Save all changes to the database
            _context.SaveChanges();

            // Log the action taken by the user
            _logger.Log(LogLevel.Information, $"User {GetUserPrincipalFromContext().Identity.Name} set all students to not in school");

            // Return success message
            return Ok("Users set to not in school");
        }



        [HttpPost("Absence/Write")]
        [AllowAnonymous]
        public async Task<IActionResult> AbsenceWrite([FromBody] string Isic) // need this one for the late time logic || gotta finish leaving school early
        {
            //gets users isic based on isic scanned with RFID
            var isic = _context.Isics
                .SingleOrDefault(s => s.IsicId == Isic);
            if (isic == null)
            {
                _logger.Log(LogLevel.Warning, $"Isic {Isic} was not found");
                return BadRequest("Isic not found");
            }
            //gets user based on user id stored in isic
            var user = _context.Users
                .SingleOrDefault(User => User.Id == isic.UserId);
            if (user == null)
            {
                _logger.Log(LogLevel.Warning, $"User with Isic {Isic} was not found");
                return BadRequest("User with this isic wasnt found");
            }
            var hours = GetLessonList(DateTime.Today, user).Result;




            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) >= (hours.Last().EndTimeInMinutes - 10) && user.InSchool == true)
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} left");
                return Ok("Student left");
            }
            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) < (hours.First().StartTimeInMinutes + 5) && user.InSchool == false) // if user isnt in school and isnt late based on start time of the first hour in students schedule
            {
                //Saves student into in school DB (doesnt write him any absence)
                user.InSchool = true;
                _context.Update(user);
                _context.SaveChanges();
                _logger.Log(LogLevel.Information, $"Student {user.UserName} arrived in time");
                return Ok();
            }
            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) > (hours.First().StartTimeInMinutes + 5) && user.InSchool == false) // if user isnt in school and is late based on start time of the first hour
            {

                var content = new Absence { UserId = isic.UserId, TimeOfArrival = DateTime.UtcNow.AddHours(2), Excused = false, Date = DateTime.UtcNow.AddHours(2).Date }; //inicializes new absence for the student

                _context.Absences.Add(content); // adds absence to DB

                user.InSchool = true; //sets user as in school

                _context.Users.Update(user); //updates user in DB


                _context.SaveChanges();
                _logger.Log(LogLevel.Information, $"Student {user.UserName} is late");
                return Ok("Absence added");

            }

            if (((int)DateTime.UtcNow.AddHours(2).TimeOfDay.TotalMinutes) < (hours.Last().EndTimeInMinutes - 10) && user.InSchool == true) // if user is in school and leaves early
            {
                var content = new Absence { UserId = isic.UserId, TimeOfExit = DateTime.UtcNow.AddHours(2), Excused = false, Date = DateTime.UtcNow.AddHours(2).Date }; //inicializes new absence for the student

                _context.Absences.Add(content); // adds absence to DB

                user.InSchool = false; //sets user as in school

                _context.Users.Update(user); //updates user in DB


                _context.SaveChanges();
                _logger.Log(LogLevel.Information, $"Student {user.UserName} left early");
                return Ok("Absence added");
            }



            return BadRequest();

        }

        [HttpPost("Absence/Excuse")]
        public async Task<IActionResult> ExcuseAbsence([FromBody] List<AbsenceExcuseVM> content, int pin) //Endpoint for excusing, takes list of dates and reasons and 1 pin code 
        {

            // Retrieve user from the database based on the user ID from the HTTP context
            var user = _context.Users.SingleOrDefault(s => s.Id == GetUserPrincipalFromContext().GetUserId());
            if (user == null)
            {
                _logger.Log(LogLevel.Information, "User not found in database");
                return NotFound();
            }



            // Validate PIN hash against the user's stored PIN hash
            if (user.ParentPin == pin.ToString())
            {
                // Iterate through each provided absence excuse
                for (int i = 0; i < content.Count; i++)
                {
                    var date = content[i].Date.Date.AddHours(2).ToUniversalTime().Date;
                    // Attempt to find the specific unexcused absence in the database
                    var absence = await _context.Absences.FirstOrDefaultAsync(s => s.UserId == user.Id && s.Date.Value.Date == date && !s.Excused);
                    if (absence == null)
                    {
                        _logger.Log(LogLevel.Information, $"No unexcused absence found for date {content[i].Date}");
                        return BadRequest("Absence doesn't exist");
                    }

                    // Mark the absence as excused and add the reason
                    absence.Excused = true;
                    absence.Reason = content[i].Reason;
                    _context.Update(absence);
                }

                // Save all changes to the database
                _context.SaveChanges();
                _logger.Log(LogLevel.Information, $"Absences successfully excused for {user.UserName}");
                return Ok("Successfully excused");
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Incorrect PIN provided");
                return BadRequest("Incorrect PIN");
            }


        }




        [HttpGet("Set/UserInSchool")]
        [Authorize(Policy = Policies.TEACHER)]
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
            return Ok("User In School changed to " + user.InSchool);
        }




        [HttpGet("Absences/Get")]
        public async Task<IActionResult> GetAbsences() //Gets users absence including indexes of missed hours
        {
            var user = _context.Users.SingleOrDefault(s => s.Id == GetUserPrincipalFromContext().GetUserId());
            if (user == null) // if user isnt found
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} wasnt found");
                return NotFound();
            }

            var absences = _context.Absences.Where(s => s.UserId == user.Id).OrderBy(s => s.TimeOfArrival).ToList();
            if (absences == null)
            {
                _logger.Log(LogLevel.Information, $"User {user.UserName} has no absence");

                return Ok("No absence found");
            }
            var today = DateTime.Now;
            //iterates through absences and puts them into one VM for easier sending
            if (today.Month >= 9 || today.Month <= 1)
            {
                // First half-year logic (Second Monday of September to last day of January)

                _logger.Log(LogLevel.Information, $"User {user.UserName} absence succesfully sent");

                return Ok(absences.Where(s => s.Date.Value.Month <= 1 || s.Date.Value.Month >= 9).ToList());
            }
            else
            {
                // Second half-year logic (First of February to a week before the end of June)

                _logger.Log(LogLevel.Information, $"User {user.UserName} absence succesfully sent");
                return Ok(absences.Where(s => !(s.Date.Value.Month <= 1) || !(s.Date.Value.Month >= 9)).ToList());

            }


           

           
        }

        [HttpGet("Get/AbsenceScore")]
        public async Task<IActionResult> GetStudentAbsenceScore() // calculates the percentage of student's missed hours
        {
            // Retrieve current user from context
            var user = GetUserFromContext();
            _logger.Log(LogLevel.Information, $"Calculating absence score for user {user.UserName}");

            // Get subjects associated with the user
            var subjects = GetUserSubjects(user).Result;

            // List to hold absence scores
            var score = new List<AbsenceScoreVM>();

            // Get all Mondays for the current half of the year
            var hours = GetMondaysForCurrentHalfOfYear();

            // List to store all school days in the week
            var allDays = new List<DateTime>();
            foreach (var hour in hours)
            {
                for (int i = 0; i < 5; i++) // Add weekdays starting from Monday
                {
                    allDays.Add(hour.AddDays(i));
                }
            }

            // List to hold all lessons in those days
            var allHours = new List<Lesson>();
            foreach (var day in allDays)
            {
                var lessonList = GetLessonList(day, user).Result;
                if (lessonList == null)
                {
                    _logger.Log(LogLevel.Warning, $"No lessons found for day {day.ToShortDateString()}");
                    continue;
                }
                allHours.AddRange(lessonList);
            }

            // Count lessons by name
            var countByName = allHours
                                .GroupBy(entry => entry.Name)
                                .Select(group => new { Name = group.Key, Count = group.Count() });

            // Retrieve all missed lessons for the student
            var studentMissedHours = GetAllStudentMissedHours(user.UserName).Result;
            var countByNameMissed = studentMissedHours
                                    .GroupBy(entry => entry.Name)
                                    .Select(group => new { Name = group.Key, Count = group.Count() });

            // Calculate missed hours percentage per subject
            foreach (var subject in subjects)
            {
                var all = countByName.FirstOrDefault(s => s.Name == subject.Name);
                var missed = countByNameMissed.FirstOrDefault(s => s.Name == subject.Name);
                Console.WriteLine();
                if (missed == null)
                {
                    score.Add(new AbsenceScoreVM { Name = subject.Name, Score = 0 });
                    continue;

                }
                var missedPercentage = (double)missed.Count / all.Count * 100;
                score.Add(new AbsenceScoreVM { Name = subject.Name, Score = missedPercentage });
            }

            _logger.Log(LogLevel.Information, $"Absence score calculated for user {user.UserName}");
            return Ok(score);
        }



        [HttpGet("Generate/ParentPin")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> GenerateParentPins() //Generates parent pins to all students
        {
            var users = _context.Users.Where(s => s.ClassId != null);
            _logger.Log(LogLevel.Information, $"Starting to generate parent pins to all students");
            var r = new Random();
            foreach (var user in users)
            {
                user.ParentPin = r.Next(1001, 9999).ToString();
                _context.Update(user);
            }
          
            await _context.SaveChangesAsync();
            _logger.Log(LogLevel.Information, $"Pins generated for every student");
            return Ok("Pins succesfully generated");

        }



        [HttpPost("Generate/ParentPin/SpecificUser")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> GenerateParentPinsForSpecificUser([FromBody] string userName)
        {
            var user = _context.Users.FirstOrDefault(s => s.UserName.ToUpper() == userName.ToUpper());
            var r = new Random();
            if (user == null) // if user isnt found
            {
                _logger.Log(LogLevel.Information, $"Student {userName} wasnt found");
                return BadRequest("User not found");
            }
            user.   ParentPin = r.Next(1000, 9999).ToString();
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok("User parent pin generated " + user.ParentPin);

        }



        [HttpPost("GetUserParentPin")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> GetUserParentPin([FromBody] string username) // Sends users parent pin based on his username
        {
            var user = _context.Users.FirstOrDefault(s => s.UserName == username);
            if (user == null)
            {
                _logger.LogInformation($"User {username} wasnt found");
                return BadRequest("User not found");


            }
            _logger.LogInformation($"Succesfully returned pin");
            return Ok(user.ParentPin);
        }


        private User GetUserFromContext()
        {
            return _context.Users.Single(s => GetUserPrincipalFromContext().Identity.Name == s.UserName);
        }

        private ClaimsPrincipal GetUserPrincipalFromContext()
        {
            var user = _signInManager.Context.User;


            _ = _contextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
            _logger.Log(LogLevel.Information, $"User {user.Identity.Name} request principal from context");
            return _contextAccessor.HttpContext.User;
        }

   

        public static int? TimeOfDayToMinutes(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            return dateTime.Value.Hour * 60 + dateTime.Value.Minute;
        }

        private bool absenceCheck(int? arrival, int? exit, Lesson lesson, bool shouldBePresent)
        {
            if (!shouldBePresent) return false;
            if (exit == null && arrival > lesson.StartTimeInMinutes) return true;
            if (arrival == null && exit < lesson.EndTimeInMinutes) return true;

            return arrival >= lesson.EndTimeInMinutes || exit <= lesson.StartTimeInMinutes;
        }

        private async Task<List<Subject>> GetUserSubjects(User user)
        {
            var userSubjects = await _context.StudentSubjects.Where(s => s.StudentId == user.Id).ToListAsync();
            _logger.Log(LogLevel.Information, $"Retrieved {userSubjects.Count} subjects for user {user.UserName}");
            // Initialize a list to collect subjects.
            var subjects = new List<Subject>();

            // Iterate over each student subject to fetch the subject from the database.
            foreach (var subject in userSubjects)
            {
                var current = _context.Subjects.FirstOrDefaultAsync(s => s.Id == subject.SubjectId).Result;

                subjects.Add(current);
            }
            return subjects;
        }

        private async Task<List<Lesson>> GetLessonList(DateTime Date, User user)
        {
            var hours = new List<Lesson>();
            // Attempt to find all altered schedules and takes only the one valid for the day
            AlteredSchedule? schedule = null;
            try
            {
                var schedules = await _context.AlteredSchedules
                    .Where(s => s.ClassId == user.ClassId)
                    .ToListAsync();

                schedule = schedules.FirstOrDefault(s => s.Date.Date == Date.Date);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error retrieving altered schedule for user {user.UserName} on {Date}: {ex.Message}");
            }

            // Log the schedule finding result.
            if (schedule == null)
            {
                _logger.Log(LogLevel.Information, $"No altered schedule found for date {Date.Date}, using regular schedule.");
                var currentDay = Date.Date.DayOfWeek.ToString();
                if (Date.Date.DayOfWeek.ToString() == "Satuday" || Date.Date.DayOfWeek.ToString() == "Sunday")
                {
                    return null;
                }
                var sched = _context.Schedules.First(Day => Day.ClassId == user.ClassId && Day.Day == currentDay);
                hours = _context.Lessons.Where(s => s.ScheduleId == sched.Id).OrderBy(l => l.LessonIndex).ToList();
            }
            else
            {
                _logger.Log(LogLevel.Information, $"Using altered schedule for date {Date.Date}");
                hours = _context.Lessons.Where(s => s.ScheduleId == schedule.Id).OrderBy(l => l.LessonIndex).ToList();
            }

            return hours;
        }
        private static List<DateTime> GenerateMondays(DateTime schoolYearStart, DateTime schoolYearEnd)
        {
            List<DateTime> mondays = new List<DateTime>();

            TimeSpan duration = schoolYearEnd - schoolYearStart;


            // Find the first Monday on or after the start date
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)schoolYearStart.DayOfWeek + 7) % 7;
            DateTime firstMonday = schoolYearStart.AddDays(daysUntilMonday);

            // Loop to add all Mondays until the midpoint of the school year
            for (DateTime date = firstMonday; date <= schoolYearEnd; date = date.AddDays(7))
            {
                mondays.Add(date);

            }

            return mondays;
        }
        public static List<DateTime> GetMondaysForCurrentHalfOfYear()
        {
            DateTime today = DateTime.Today;
            DateTime halfYearStart;
            DateTime halfYearEnd;

            // Determine the current academic half based on today's date
            if (today.Month >= 9 || today.Month <= 1)
            {
                // First half-year logic (Second Monday of September to last day of January)
                int year = today.Month >= 9 ? today.Year : today.Year - 1;
                DateTime september = new DateTime(year, 9, 1);
                halfYearStart = GetSecondMondayOfSeptember(september);
                halfYearEnd = new DateTime(year + 1, 1, 31);
            }
            else
            {
                // Second half-year logic (First of February to a week before the end of June)
                int year = today.Year;
                halfYearStart = new DateTime(year, 2, 1);
                halfYearEnd = new DateTime(year, 6, 30).AddDays(-7);
            }

            // Ensure that we do not generate dates beyond today or the half-year end
            DateTime endDate = (today < halfYearEnd) ? today : halfYearEnd;

            return GenerateMondays(halfYearStart, endDate);
        }

        private static DateTime GetSecondMondayOfSeptember(DateTime september)
        {
            // Calculate the date of the first Monday in September
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)september.DayOfWeek + 7) % 7;
            DateTime firstMonday = september.AddDays(daysUntilMonday);
            // Second Monday will be exactly one week after the first
            return firstMonday.AddDays(7);
        }
        private async Task<List<Lesson>> GetAllStudentMissedHours(string userName) //kinda works but has to
        {

            var user = _context.Users.FirstOrDefault(s => s.UserName.ToUpper() == userName.ToUpper());
            if (user == null) // if user isnt found
            {
                _logger.Log(LogLevel.Information, $"Student {userName} wasnt found");
                return null;
            }
            var today = DateTime.Now;
            var halfYearStart = new DateTime();
            var halfYearEnd = new DateTime();

            if (today.Month >= 9 || today.Month <= 1)
            {
                // First half-year logic (Second Monday of September to last day of January)
                int year = today.Month >= 9 ? today.Year : today.Year - 1;
                DateTime september = new DateTime(year, 9, 1);
                halfYearStart = GetSecondMondayOfSeptember(september);
                halfYearEnd = new DateTime(year + 1, 1, 31);
            }
            else
            {
                // Second half-year logic (First of February to a week before the end of June)
                int year = today.Year;
                halfYearStart = new DateTime(year, 2, 1);
                halfYearEnd = new DateTime(year, 6, 30).AddDays(-7);
            }


            // Gets all user absences


            var subjects = GetUserSubjects(user).Result;

            // Retrieve all absences for the user and log the count.
            var absenceList2 = _context.Absences.Where(s => user.Id == s.UserId).ToList();

            var absenceList = _context.Absences.Where(s => user.Id == s.UserId && halfYearStart.Date.ToUniversalTime() <= s.Date.Value.ToUniversalTime() && s.Date.Value.ToUniversalTime() <= halfYearEnd.Date.ToUniversalTime()).ToList();
            _logger.Log(LogLevel.Information, $"Found {absenceList.Count} absences for user {user.UserName}");

            if (absenceList.Count == 0)
            {
                _logger.Log(LogLevel.Information, $"Student {user.UserName} has no absences");
                return null;
            }
            var MissedHours = new List<Lesson>();
            // Handle schedules and absences to determine missed lessons.
            
            foreach (Absence absence in absenceList2)
            {
                if (!(halfYearStart <= absence.Date && halfYearEnd >= absence.Date))
                {
                    continue;
                }

                //Get all the hours for the day
                var hours = GetLessonList(absence.Date.Value, user).Result;

                // Check missed hours based on user's absence details.
                for (int i = 0; i < hours.Count; i++)
                {
                    bool studentShouldBePresent = subjects.Any(sub => sub.Name == hours[i].Name);
                    int? AbsenceArrivalInMinutes = TimeOfDayToMinutes(absence.TimeOfArrival);
                    int? AbsenceExitinMinutes = TimeOfDayToMinutes(absence.TimeOfExit);
                    if (absenceCheck(AbsenceArrivalInMinutes, AbsenceExitinMinutes, hours[i], studentShouldBePresent))
                    {
                        MissedHours.Add(hours[i]);

                    }
                }

            }

            _logger.Log(LogLevel.Information, $"Absence processing complete for user {user.UserName}");
            return MissedHours;

        }
        
        public static DateTime[] GetWeekDays(DateTime date)
        {
            // Assuming the first day of the week is Sunday.

            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

            // Calculate the first day of the week.
            DateTime firstDay = date.AddDays(-((int)date.DayOfWeek - (int)firstDayOfWeek));

            // Generate an array of DateTime objects representing each day of the week.
            DateTime[] weekDays = new DateTime[5];
            for (int i = 0; i < 5; i++)
            {
                weekDays[i] = firstDay.AddDays(i);
            }

            return weekDays;
        }




    }
}
