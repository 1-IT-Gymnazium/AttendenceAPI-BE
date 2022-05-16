namespace AttendenceApi.ViewModels
{
    public class LoggedUserVm
    {
        public string UserName { get; set; } = null!;

        public bool IsAuthenticated { get; set; }
    }
}
