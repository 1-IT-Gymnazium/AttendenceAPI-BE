



using AttendenceApi.Controllers;

namespace AttendenceApi.Data.Seeds
{
    public class ClassSeed
    {
        public static async Task CreateClass(AppDbContext dbContext)
        {
            var clas = new Classes
            {
                Id = AuthController.GuidFromString("4.T"),
                Name = "4.T"

            };
            var isindb = dbContext.Classes.FirstOrDefault(c => c.Name == clas.Name);
            if (isindb == null)
            {
                var result = await dbContext.Classes.AddAsync(clas);

                await dbContext.SaveChangesAsync();
            }
            

        }
    }
}
