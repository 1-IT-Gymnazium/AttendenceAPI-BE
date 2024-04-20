using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.Utils;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

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






        [HttpPost("New/Teacher")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> NewTeacher([FromBody] NameVM VM) // adding teachers claims based on First and Last name //Needs Test
        {
            _logger.LogInformation("adding new teacher claim");
            var user = _context.Users.First(s => s.FirstName == VM.FirstName && VM.LastName == s.LastName);
            if (user == null)
            {
                _logger.LogError("Teacher with this name wasnt found");
                return BadRequest("User doesnt exist");
            }
            _context.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = user.Id, ClaimValue = Claims.TEACHER, ClaimType = Claims.TEACHER });
            _context.SaveChanges();
            _logger.LogInformation("Claim succesfully added");
            return Ok("Teacher Claim Added");
        }


        [HttpPost("Remove/Teacher")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> RemoveTeacher([FromBody] NameVM VM) // removing teachers claims based on First and Last name // needs test
        {
            _logger.LogInformation("removing teacher claim");
            var user = _context.Users.First(s=> s.FirstName == VM.FirstName && VM.LastName == s.LastName);

            if (user == null)
            {
                _logger.LogError("Teacher with this name wasnt found");
                return BadRequest("User doesnt exist");
            }
            var claims = _context.UserClaims.Where(s=> s.ClaimType == Claims.TEACHER && s.UserId == user.Id);

            foreach (var claim in claims)
            {
                _context.UserClaims.Remove(claim);
            }
            _context.SaveChanges();
            _logger.LogInformation("Teacher claims removed");
            return Ok("Teacher claim removed");
        }

 
        



    }
}
