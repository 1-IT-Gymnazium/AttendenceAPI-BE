



using AttendenceApi.Controllers;

namespace AttendenceApi.Data.Seeds
{
    public class ClassSeed
    {
        public static async Task CreateClass(AppDbContext dbContext)
        {
            var clas = new Classes
            {
                Id = AuthController.GuidFromString("1.A"),
                Name = "1.A"

            };
            var isindb = dbContext.Classes.FirstOrDefault(c => c.Name == "1.A");
            if (isindb == null)
            {
                var result = await dbContext.Classes.AddAsync(clas);

                await dbContext.SaveChangesAsync();
            }
            

        }
    }
}
