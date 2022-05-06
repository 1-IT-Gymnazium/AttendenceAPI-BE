using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Policy;

namespace AttendenceApi.Services
{
    public interface IUserService 
    {
        Task<bool> LoginAsync(LoginViewModel model);
        Task<IdentityResult> CreateUser(User user, string password);
    }
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager = null!;
        private readonly AppDbContext _context = null !;
        private readonly ILogger<UserService> _logger;
        public UserService(SignInManager<User> signInManager, AppDbContext context,ILogger<UserService> logger)
        {
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }
        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded == true)
            {
                _logger.LogInformation("User logged in");
                var User = _context.Users.Single(s => s.UserName == model.Email);

                return result.Succeeded;
            }

            return result.Succeeded;
        }
        public async Task<IdentityResult> CreateUser(User user, string password)
        {

            return await _signInManager.UserManager.CreateAsync(user, password);
        }

    }
}
