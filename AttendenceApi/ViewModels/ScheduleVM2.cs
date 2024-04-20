using AttendenceApi.Data;

namespace AttendenceApi.ViewModels
{
    public class ScheduleVM2
    {
        public Schedule? schedule { get; set; }
        public AlteredSchedule? alteredSchedule { get; set; }
        public List<Lesson> lessons { get; set; }
    }
}
