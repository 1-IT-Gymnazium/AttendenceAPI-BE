using AttendenceApi.Data.Indentity;

namespace AttendenceApi.ViewModels
{
    public class LessonVm
    {
       
        public string Name { get; set; } = null!;
        public Guid ScheduleId { get; set; }

        public User Teacher { get; set; } = null!;
       public int? StartTimeInMinutes { get; set; }
        public int? EndTimeInMinutes { get; set; }
        public int LessonIndex { get; set; }


    }
}
