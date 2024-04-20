using AttendenceApi.Data.Indentity;

namespace AttendenceApi.Data
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ScheduleId { get; set; }
        public Schedule Schedule { get; set; } = null!;
        public Guid? AlteredScheduleId {  get; set; }
        public AlteredSchedule AlteredSchedule { get; set; }
        public string? Parity { get; set; }
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
        public int LessonIndex { get; set; }
        public int? StartTimeInMinutes { get; set; }
        public int? EndTimeInMinutes { get; set; }
        public string Room { get; set; }



    }
}
