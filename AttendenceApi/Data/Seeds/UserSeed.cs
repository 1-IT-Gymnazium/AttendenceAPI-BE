using AttendenceApi.Controllers;
using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.NewFolder
{
    public static class UserSeed
    {
        public static async Task  CreateAdmin(UserManager<User> userManager,AppDbContext dbContext)
            
        {
            var admin = new User()
            {
                UserName = "User123",
                Email = "Example@Example.com",
                InSchool = false,
                ClassId = AuthController.GuidFromString("1.A")



            };
            
            var user = await userManager.FindByEmailAsync(admin.Email);
            if (user == null)
            {
                var created = await userManager.CreateAsync(admin, "Test123");
                if (created.Succeeded)
                {
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Something went wrong in creating of user.\n{string.Join("\n", created.Errors.Select(x => x.Description))}");
                }
            }
        }
    }
}
