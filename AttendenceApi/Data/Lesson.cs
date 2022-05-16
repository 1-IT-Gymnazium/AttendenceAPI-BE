using AttendenceApi.Data.Indentity;

namespace AttendenceApi.Data
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
        public int LessonIndex { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


    }
}
