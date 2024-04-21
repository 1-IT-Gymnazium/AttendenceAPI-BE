
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
        } // get user info from context

        [HttpGet("Logout")]
        public async Task<IActionResult> LogOut() // uses sign in manager to logout
        {
           
            await _signInManager.SignOutAsync();
            return Ok("User successfully signed out");
        }
    


        
        [Tags("Authentications")]
        [HttpPost("AddUserAdminClaim")]
        [Authorize(Policy = Policies.SUPERADMIN)]
        public async Task<IActionResult> AddUserAdminClaim([FromBody] LoggedUserVm UserToAdd) //adding admin claim to user
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == UserToAdd.UserName);
            if (user == null)
            {
                return BadRequest("UserNotFound");
            }
            var result = _userManager.AddClaimAsync(user, _adminClaim).Result;
            return Ok(result);
        }



        [HttpPost("logindb")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsyncc(LoginViewModel model)
        {
            // Attempt to connect to LDAP with provided credentials
            
            _logger.LogInformation($"Attempting LDAP authentication for user {model.Name}");
            var directoryEntry = new DirectoryEntry("LDAP://10.129.0.12", model.Name, model.Password);
            var directorySearcher = new DirectorySearcher(directoryEntry);
            try
            {
                var result = directorySearcher.FindAll();
            }
            catch (DirectoryServicesCOMException ex)
            {
                _logger.LogWarning($"LDAP authentication failed for user {model.Name}: {ex}");
                return NotFound("User or password wrong");
            }

            // Check if the user exists in the local database
            _logger.LogInformation($"Checking local database for user {model.Name}");
            var user = _context.Users.SingleOrDefault(x => x.UserName == model.Name);
            if (user == null)
            {
                _logger.LogInformation($"User {model.Name} was not found in the database");
                return Ok("UserNotInDb");
            }

            // Create a principal for the user and sign them in
            _logger.LogInformation($"Signing in user {model.Name}");
            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            await HttpContext.SignInAsync(userPrincipal);

            // Retrieve user claims to determine role
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(x => x.Type == _adminClaim.Type))
            {
                _logger.LogInformation($"Admin user {model.Name} logged in successfully");
                return Ok("Admin");
            }
            if (claims.Any(x => x.Type == Claims.TEACHER))
            {
                _logger.LogInformation($"Teacher user {model.Name} logged in successfully");
                return Ok("Teacher");
            }
            if (claims.Any(x => x.Type == _userClaim.Type))
            {
                _logger.LogInformation($"Student user {model.Name} logged in successfully");
                return Ok("Student");
            }

            // Handle case where no relevant claims were found
            _logger.LogWarning("User {UserName} does not have the required claims", model.Name);
            return NotFound("UserDoesntHaveAClaim");


        }

        
        [HttpPost("UserIsic")]
        [Authorize(Policy = Policies.TEACHER)]
        public async Task<IActionResult> UserIsic([FromBody] string Isic)
        {
            // Attempt to retrieve user ID from the current context
            _logger.LogInformation("Attempting to retrieve user ID from context for ISIC registration.");
            var userId = TryGetUserIdFromContext();
            if (!userId.HasValue)
            {
                _logger.LogWarning("Failed to retrieve user ID from context.");
                return BadRequest("User ID is required.");
            }

            // Log the creation of a new ISIC record
            _logger.LogInformation($"Creating new ISIC record for user ID {userId.Value} with ISIC ID {Isic}.");
            var userIsic = new Isic
            {
                UserId = userId.Value,
                IsicId = Isic
            };

            // Add the new ISIC record to the database
            _context.Isics.Add(userIsic);
            _logger.LogDebug("New ISIC record added to context.");

            // Save changes to the database
            await _context.SaveChangesAsync();
            _logger.LogInformation("Changes saved to the database successfully.");

            // Return a success response
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
