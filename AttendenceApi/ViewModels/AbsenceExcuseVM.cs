using System.ComponentModel.DataAnnotations;

namespace AttendenceApi.ViewModels
{
    public class AbsenceExcuseVM
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Reason { get; set; } = null!;
        
    }
}
