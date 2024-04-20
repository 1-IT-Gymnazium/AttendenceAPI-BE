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
                ClassId = AuthController.GuidFromString("1.A"),
                FirstName = "More",
                LastName = "Pokusnik",



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
            var utilUser = new User()
            {
                UserName = "test1",
                Email = "test1@email.com",
                InSchool = false,
                ClassId = AuthController.GuidFromString("3.T"),
                FirstName = "Janek",
                LastName= "Rubeš",
            };

            user = null;
            user = await userManager.FindByNameAsync(utilUser.UserName);

            if (user == null)
            {
                var create = await userManager.CreateAsync(utilUser, "Password123");
                if (create.Succeeded)
                {
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Something went wrong in creating of user.\n{string.Join("\n", create.Errors.Select(x => x.Description))}");
                }
            }
            var jirikUser = new User()
            {
                UserName = "1024",
                Email = "1024@student.itgymnazium.cz",
                InSchool = false,
                ClassId = AuthController.GuidFromString("4.T"),
                FirstName = "Jiří",
                LastName = "Drbohlav",
            };

            user = null;
            user = await userManager.FindByNameAsync(jirikUser.UserName);

            if (user == null)
            {
                var create = await userManager.CreateAsync(jirikUser, "MasterPassword123");
                if (create.Succeeded)
                {
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Something went wrong in creating of user.\n{string.Join("\n", create.Errors.Select(x => x.Description))}");
                }
            }
        }
    }
}
