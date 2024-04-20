using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.Utils;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendenceApi.Controllers
{
    public class SubjectController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public SubjectController(IUserService userService, AppDbContext context, ILogger<AuthController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }



        [HttpPost("SetSubjectToUser")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult SetSubjectToUser([FromBody] SubjectUserVM usercode) //sets subject to a specific user
        {

            User user = _context.Users.FirstOrDefault(s => s.UserName == usercode.UserName);
            if (user == null)
            {
                _logger.Log(LogLevel.Information, $"User {usercode.UserName} wasnt found");
                return BadRequest("UserNotFound");
            }

            Subject subject = _context.Subjects.FirstOrDefault(s => s.Name == usercode.SubjectName);
            if (subject == null)
            {
                _logger.Log(LogLevel.Information, $"Subject {usercode.SubjectName} wasnt found");
                return BadRequest("Subject doesnt exist, add it first");
            }
            var subjectuser = new StudentSubject { StudentId = user.Id, SubjectId = subject.Id };
            if (_context.StudentSubjects.FirstOrDefault(s => s.StudentId == subject.Id && s.StudentId == user.Id) != null)
            {
                return Ok("UserAlreadyHasThisSubject");
            }
            _context.StudentSubjects.Add(subjectuser);
            _context.SaveChanges();

            return Ok("UserSubjectAdded");
        }


        [HttpGet("Get/Subjects")]
        public IActionResult GetAllSubjects() //sends all subjects
        {
            _logger.Log(LogLevel.Information, "All subjects gotten");
            return Ok(_context.Subjects.Where(s => 1 == 1));
        }


        [HttpPost("Add/Subject")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult AddSubject([FromBody] string subjectname) //adds subject to DB
        {
            var subject = new Subject { Name = subjectname };
            if (_context.Subjects.SingleOrDefault(s => s.Name == subject.Name) != null)
            {
                _logger.Log(LogLevel.Information, $"Subject {subjectname} already in DB");
                return BadRequest("Subject already in DB");
            }
            _context.Subjects.Add(subject);
            _context.SaveChanges();
            _logger.Log(LogLevel.Information, $"Subject {subjectname} added to DB");
            return Ok("Subject Added");
        }


        [HttpPost("Add/SubjectToClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> AddSubjectToClass([FromBody] SubjectClassVM data)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Name == data.Subject);
            if (subject == null)
            {
                return BadRequest("Subject not in DB, add it first");
            }
            var Class = _context.Classes.FirstOrDefault(s => s.Name == data.Class);
            if (Class == null)
            {
                return BadRequest("Class not found");
            }
            var students = _context.Users.Where(s => s.ClassId == Class.Id);
            if (students == null)
            {
                return BadRequest("No students found");
            }
            foreach (var student in students)
            {
                _context.StudentSubjects.Add(new StudentSubject { StudentId = student.Id, SubjectId =subject.Id});

            }
            _context.SaveChanges();
            return Ok("Subject added to class");

        }
        [HttpPost("Remove/Subject/FromClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> RemoveSubjectFromClass([FromBody] SubjectClassVM data)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Name == data.Subject);
            var subs = new List<StudentSubject>();
            if (subject == null)
            {
                return BadRequest("Subject not in DB, add it first");
            }
            var Class = _context.Classes.FirstOrDefault(s => s.Name == data.Class);
            if (Class == null)
            {
                return BadRequest("Class not found");
            }
            var students = await  _context.Users.Where(s => s.ClassId == Class.Id).ToListAsync();
            if (students == null)
            {
                return BadRequest("No students found");
            }
            foreach (var student in students)
            {
               var sub = await _context.StudentSubjects.FirstOrDefaultAsync(s => s.SubjectId == subject.Id);
                subs.Add(sub);
            }
            foreach(var s in subs)
            {
                _context.StudentSubjects.Remove(s);
               _context.SaveChangesAsync().Wait();
            }
            
            return Ok("Subject removed from all class");

        }
        [HttpPost("Remove/Subject/FromUser")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> RemoveSubjectFromUser([FromBody] SubjectUserVM data)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Name == data.SubjectName);
            var subs = new List<StudentSubject>();
            if (subject == null)
            {
                return BadRequest("Subject not in DB, add it first");
            }

            var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == data.UserName);
            if (user == null)
            {
                return BadRequest("No students found");
            }
            var sub = await _context.StudentSubjects.Where(s => s.SubjectId == subject.Id && s.StudentId == user.Id).ToListAsync();
            foreach (var subejc in sub)
            {
                _context.StudentSubjects.Remove(subejc);
                _context.SaveChangesAsync().Wait();
            }

       

            return Ok("Subject removed from student");

        }
    }
}
