
using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using AttendenceApi.Services;
using AttendenceApi.Utils;
using AttendenceApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace AttendenceApi.Controllers
{
    [Authorize]
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
        private readonly Claim _userClaim;
        private readonly Claim _adminClaim;

        public AuthController(IUserService userService, AppDbContext context, ILogger<AuthController> logger, IHttpContextAccessor contextAccessor, UserManager<User> user, SignInManager<User> signInManager)
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

        [Tags("Authentications")]
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

        /*
        [Tags("Authentications")]
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
                return ValidationProblem(ModelState)22;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "LOGIN_FAILED");
                return ValidationProblem(ModelState);
            }

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

            await HttpContext.SignInAsync(userPrincipal);
            var claims = await _userManager.GetClaimsAsync(user);

            
            if (claims[0].Value == _userClaim.Value)
            {
                return Ok("Student");
            }
            if (_userManager.GetClaimsAsync(user).Result.Contains(_adminClaim))
            {
                return Ok("Admin");
            }
            if (_userManager.GetClaimsAsync(user) == null)
            {
                return BadRequest("TooBad");
            }



            return BadRequest();

        } */

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateVM model)
        {

            var classId = GuidFromString(model.ClassId.ToUpper());
            var user = new User { ClassId = classId, Email = model.Email, InSchool = false, UserName = model.UserName };

            var result = await _userService.CreateUser(user, model.Password);

            await _userManager.AddClaimAsync(user, _userClaim);
            return Ok(result);

        }



        [Authorize(Policy = Policies.SUPERADMIN)]
        [Tags("Authentications")]
        [HttpPost("AddUserAdminClaim")]
        public async Task<IActionResult> AddUserAdminClaim([FromBody] LoggedUserVm UserToAdd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == UserToAdd.UserName);
            if (user == null)
            {
                return BadRequest("UserNotFound");
            }
            var result = _userManager.AddClaimAsync(user, _adminClaim).Result;
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost("AddUserInDB")]
        public async Task<IActionResult> AddUserInDB([FromBody] RegisterVM model)
        {
            
            var directoryEntry = new DirectoryEntry("LDAP://10.129.0.12", model.Name, model.Password);
            var directorySearcher = new DirectorySearcher(directoryEntry);
            try
            {
                var result = directorySearcher.FindAll();
            }
            catch (DirectoryServicesCOMException ex)
            {
                return NotFound(ex);
            }
            

            var user = _context.Users.SingleOrDefault(x => x.UserName == model.Name);
            if (user == null)
            {
                await RegisterUser(model);
            }


            return Ok("UserAlreadyInDB");
        }

        [HttpPost("logindb")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsyncc(LoginViewModel model)
        {




            var directoryEntry = new DirectoryEntry("LDAP://10.129.0.12", model.Name, model.Password);
            var directorySearcher = new DirectorySearcher(directoryEntry);
            try
            {
                var result = directorySearcher.FindAll();
            }
            catch (DirectoryServicesCOMException ex)
            {
                return NotFound(ex);
            }


            var user = _context.Users.SingleOrDefault(x => x.UserName == model.Name);
            if (user == null)
            {
                return Ok("UserNotInDb");
            }
            

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            await HttpContext.SignInAsync(userPrincipal);
            var claims = await _userManager.GetClaimsAsync(user);
            var cl = new IdentityUserClaim<Guid> { ClaimType = Claims.USER, ClaimValue = Claims.USER };
            var nvm = _userClaim;

            
            if (claims.Any(x => x.Type == _adminClaim.Type))
            {

                return Ok("Admin");
            }
            if (claims.Any(x => x.Type == nvm.Type))
            {
                return Ok("Student");
            }
            return NotFound("UserDoesntHaveAClaim");




        }


        [HttpPost("Register/NotInDB")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterVM model)
        {
            var User = new User
            {
                UserName = model.Name,
                Email = model.Email,
                InSchool = false,
                ClassId = AuthController.GuidFromString(model.ClassId),
                PinHash = model.PinHash,

            };

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            await _userManager.UpdateSecurityStampAsync(User);
            await _userManager.AddClaimAsync(User, _userClaim);

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPost("UserIsic")]
        [Authorize(Policy = Policies.SUPERADMIN)]
        public async Task<IActionResult> UserIsic([FromBody] string Isic)
        {

            var userIsic = new Isic
            {
                UserId = TryGetUserIdFromContext().Value,
                IsicId = Isic

            };
            _context.Isics.Add(userIsic);
            await _context.SaveChangesAsync();

            return Ok();

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



        public static Guid GuidFromString(string input)
        {

            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }



    }
}
