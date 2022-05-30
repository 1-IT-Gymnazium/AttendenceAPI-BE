
using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrictCode.Agrades.Identity.Api.Utils;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AttendenceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IUserService userService, AppDbContext context, ILogger<AuthController> logger, IHttpContextAccessor contextAccessor, UserManager<User> user, SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _userManager = user;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

      
        [HttpGet("UserInfo")]
        public async Task<ActionResult<LoggedUserVm>> GetUserInfo()
        {

            var principal = GetUserPrincipalFromContext();

            if (!principal.Identities.Any(x => x.IsAuthenticated))
            {
                return ValidationProblem(new ValidationProblemDetails { Detail = "Something went wrong!", Status = StatusCodes.Status400BadRequest });
            }


            var id = TryGetUserIdFromContext();
            var user = await _userManager.Users.SingleAsync(x => x.Id == id);

            return Ok(new LoggedUserVm
            {
                UserName = user.UserName,
                IsAuthenticated = true,
            });
        }

        
        [Tags("Authentications") ]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
                return ValidationProblem(ModelState);
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
                return ValidationProblem(ModelState);
            }

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

            var authProps = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true,
            };
            var claims = new List<Claim>
                {
                    new Claim("user", user.UserName),
                    new Claim("role", "Student")
                };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authProps);
            var ussser = HttpContext.User;

            var role = _context.UserRoles.Single(s => s.UserId == user.Id);
            if (role.RoleId == new Guid("18025609-4658-4282-a14e-0efa3cd293aa"))
            {
                return Ok("Student");
            }
            if (role.RoleId == new Guid("46b98e3f-6fd9-4da7-922a-6540235a9feb"))
            {
                return Ok("Admin");
            }
            if (role == null)
            {
                return BadRequest("TooBad");
            }



            return BadRequest();

        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateVM model)
        {
            var classId = GuidFromString(model.ClassId.ToUpper());
            var user = new User { ClassId = classId, Email = model.Email, InSchool = false, UserName = model.UserName };
            var result = await _userService.CreateUser(user, model.Password);
            return Ok(result);
        }
        private Guid? TryGetUserIdFromContext()
        {
            var user = GetUserPrincipalFromContext();
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }
            return user.GetUserId();
        }
        private ClaimsPrincipal GetUserPrincipalFromContext()
        {
            var user = _signInManager.Context.User;

            _ = _contextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
            return _contextAccessor.HttpContext.User;
        }



        private static Guid GuidFromString(string input)
        {

            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }
        [HttpPost("Create/Role")]
        public IActionResult CreateRole(string name)
        {
            var result = _roleManager.CreateAsync(new Role { Name = name }).Result;
            return Ok(result);
        }


    }
}
