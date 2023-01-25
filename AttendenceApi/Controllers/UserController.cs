using AttendenceApi.Data;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttendenceApi.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public UserController(IUserService userService, AppDbContext context, ILogger<AuthController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        public IActionResult StudentClassChange([FromBody] ClassChangeVM model)
        {
            var students = _context.Users.Where(s => s.ClassId == AuthController.GuidFromString(model.PreviousClass)).ToList();
            for (int i = 0; i < students.Count(); i++)
            {
                students[i].ClassId = AuthController.GuidFromString(model.NewClass); 
            }



            return Ok("Ok");
        }

    }
}
