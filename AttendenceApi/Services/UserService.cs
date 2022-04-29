using AbsenceProjektSDarou.Models.Identity;
using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager = null!;
        public UserService(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            return result.Succeeded;
        }
        public async Task<IdentityResult> CreateUser(User user, string password)
        {


            return await _signInManager.UserManager.CreateAsync(user, password);
        }

    }
}
