using System.ComponentModel.DataAnnotations;

namespace AttendenceApi.ViewModels
{
    public class CreateScheduleVM
    {


        public string ClassId { get; set; }
        public string Day { get; set; } = string.Empty;

        public string Date { get; set; }
        public int? EndTimeOfLessonsInMinutes { get; set; }
        public int? StartTimeOfLessonsInMinutes { get; set; }
        public List<LessonVmTwo> Lessons { get; set; } = null!;

    }
}
