
using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StrictCode.Agrades.Identity.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AttendenceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IUserService userService, AppDbContext context, ILogger<AuthController> logger, IHttpContextAccessor contextAccessor, UserManager<User> user, SignInManager<User> signInManager)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _userManager = user;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet("UserInfo")]
        public async Task<ActionResult<LoggedUserVm>> GetUserInfo()
        {

            var principal = GetUserPrincipalFromContext();

            if (!principal.Identities.Any(x => x.IsAuthenticated))
            {
                return ValidationProblem(new ValidationProblemDetails { Detail = "Something went wrong!", Status = StatusCodes.Status400BadRequest });
            }

            var id = TryGetUserIdFromContext();
            var user = _userManager.Users.SingleOrDefault(x => x.Id == id);

            return Ok(new LoggedUserVm
            {
                UserName = user.UserName,
                IsAuthenticated = true,
            }); 
        }


        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginAsync ([FromBody] LoginViewModel model)
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
            await HttpContext.SignInAsync(userPrincipal);

            

            var role = _context.UserRoles.Single(s => s.UserId == user.Id);
                if (role.RoleId == 2)
                {
                    return Ok(true);
                }
               if (role.RoleId == 1)
                {
                    return Ok(false);
                }

            

            return BadRequest();
          
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateVM model)
        {
            var user = new User { ClassId = model.ClassId, Email = model.Email, InSchool = false, UserName = model.UserName };
            var result = await _userService.CreateUser(user, model.Password);
            return Ok(result);
        }
        private int? TryGetUserIdFromContext()
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
            _ = _contextAccessor.HttpContext ?? throw new ArgumentNullException("HttpContextAccessor.HttpContext");
            return _contextAccessor.HttpContext.User;
        }


    }
}
