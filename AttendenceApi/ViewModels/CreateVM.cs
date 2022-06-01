namespace AttendenceApi.ViewModels
{
    public class CreateVM
    {
        public string UserName { get; set; } = null!;
        public string ClassId{ get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
