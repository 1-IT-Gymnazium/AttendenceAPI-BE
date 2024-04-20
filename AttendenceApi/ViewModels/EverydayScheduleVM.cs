namespace AttendenceApi.ViewModels
{
    public class EverydayScheduleVM
    {


        public string ClassId { get; set; }
        public string Day { get; set; } = string.Empty;


        public int? EndTimeOfLessonsInMinutes { get; set; }
        public int? StartTimeOfLessonsInMinutes { get; set; }
        public List<LessonVmTwo> Lessons { get; set; } = null!;
    }
}
