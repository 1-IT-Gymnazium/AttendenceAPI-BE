using AbsenceProjektSDarou.Models.Identity;

namespace AttendenceApi.Services
{
    public interface IUserService 
    {
        Task<bool> LoginAsync(LoginViewModel model);
    }
}
