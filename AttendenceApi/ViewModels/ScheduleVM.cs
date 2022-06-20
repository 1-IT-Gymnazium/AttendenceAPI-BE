using AttendenceApi.Data;

namespace AttendenceApi.ViewModels
{
    public class ScheduleVM
    {
      
        public Guid? ClassId { get; set; }
      
        public string Day { get; set; } = null!;
        public string Date { get; set; } = null!;

        public List<LessonVm> Lessons { get; set; } = null!;
        public int? EndTimeOfLessonsInMinutes { get; set; }
    }
}
