namespace AttendenceApi.ViewModels
{
    public class LessonVmTwo
    {
        public string Name { get; set; } = null!;
        public string? Parity {  get; set; }
        public string Room { get; set; } = null!;
        public string Teacher { get; set; } = null!;
        public int? StartTimeInMinutes { get; set; }
        public int? EndTimeInMinutes { get; set; }
        public int LessonIndex { get; set; }
    }
}
