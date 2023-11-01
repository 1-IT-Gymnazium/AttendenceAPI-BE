using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("UserFromAppScripts")]
        [AllowAnonymous]
        public async Task<IActionResult> UserAddFromAppScripts([FromBody] string data)
        {

            var lines = data.Split("\n\r");
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i].Split(",");
                var user = new User { UserName = line[0], Email = line[1], FirstName = line[2], LastName = line[3], ClassId = AuthController.GuidFromString(line[4]) };
                _context.Users.Add(user);
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
