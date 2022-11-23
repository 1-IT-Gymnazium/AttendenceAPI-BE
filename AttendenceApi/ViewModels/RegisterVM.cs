namespace AttendenceApi.ViewModels
{
    public class RegisterVM
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string ClassId { get; set; }
        public string PinHash { get; set; } = string.Empty;
        public string Email { get; set; }

       
    }
}
