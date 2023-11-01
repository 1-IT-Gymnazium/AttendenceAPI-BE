using AttendenceApi.Data.Indentity;

namespace AttendenceApi.Data
{
    public class StudentSubject
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public User Student { get; set; }
    
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
    
    }
}
