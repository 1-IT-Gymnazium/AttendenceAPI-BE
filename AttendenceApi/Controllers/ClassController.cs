using AttendenceApi.Data;
using AttendenceApi.Services;
using AttendenceApi.Utils;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendenceApi.Controllers
{
    public class ClassController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public ClassController(IUserService userService, AppDbContext context, ILogger<AuthController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }



        [HttpPost("ChangeSingleStudentClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult SingleStudentClassChange([FromBody] SingleUserClassChangeVm model) // changing class of a single student in a specific class // needs test
        {
            var student = _context.Users.First(s => s.UserName == model.UserNumber);
            if (student == null)
            {
                _logger.LogError("Student with this name wasnt found");
                return BadRequest("User doesnt exist");
            }
            student.ClassId = AuthController.GuidFromString(model.NewClass);
            _context.SaveChanges();
            _logger.LogInformation($"User {student.UserName} class changed");



            return Ok("Ok");
        }
        [HttpPost("MakeNewClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult MakeNewClass([FromBody] string newclass)
        {
            _context.Classes.Add(new Classes { Id = AuthController.GuidFromString(newclass), Name = newclass });
            
            _context.SaveChanges();
            _logger.LogInformation($"New class {newclass} was made");
            return Ok("New class made");
        }
        [HttpPost("DeleteClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult DeleteClass([FromBody] string Class)
        {
           
            var Class1 = _context.Classes.FirstOrDefault(s=> s.Name == Class);
            if (Class1 == null)
            {
                _logger.LogInformation($"Class {Class} wasnt found");
                return BadRequest("Class wasnt found");
            }
            var students = _context.Users.Where(s => s.ClassId == Class1.Id);
            if (students.Any())
            {
                _logger.LogInformation($"Class {Class} contains students");
                return BadRequest("Change students class first, Class cannot contain students before delete");
            }
            _context.Classes.Remove(Class1);
            _context.SaveChanges();
            _logger.LogInformation($"Class {Class} was deleted");
            return Ok("Class deleted");
        }



        [HttpPost("ChangeAllStudentClass")]
        [Authorize(Policy = Policies.TEACHER)]
        public IActionResult AllStudentClassChange([FromBody] ClassChangeVM model) // changing class of all students in a specific class // needs test
        {
            var newClass = _context.Classes.FirstOrDefault(s => s.Name == model.NewClass);
            var previousClass = _context.Classes.FirstOrDefault(s => s.Name == model.PreviousClass);
            if (newClass == null)
            {
                _logger.LogError( $"{model.NewClass} wasnt found");
                return BadRequest("Class wasnt found, make new class");
            }
           // if (previousClass == null)
           // {
             //   _logger.LogError( $"{model.PreviousClass} wasnt found");
               // return BadRequest("Previous class wasnt found");
            //}
            var students = _context.Users.Where(s => s.ClassId == null).ToList();
            if (students == null)
            {
                _logger.LogError("students with this class werent found");
                return BadRequest("Class doesnt exist");
            }
            for (int i = 0; i < students.Count(); i++)
            {
                students[i].ClassId = AuthController.GuidFromString(model.NewClass);
                _logger.LogInformation($"user {students[i].UserName} class changed");
            }
            _context.SaveChanges();
            _logger.LogInformation("Classes changed");


            return Ok("Ok");
        }

    }
}
