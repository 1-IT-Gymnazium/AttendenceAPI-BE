using AttendenceApi.Data;

namespace AttendenceApi.ViewModels
{
    public class AbsenceIndexVM
    {
        public Absence Absence { get; set; }
        public List<int> LessonIndexes { get; set; }
    }
}
