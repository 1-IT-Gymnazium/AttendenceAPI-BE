using AttendenceApi.Data.Indentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class Absence
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public bool Excused { get; set; }
        public DateTime Date { get; set; }
        public DateTime? TimeOfArrival { get; set; }
        public DateTime? TimeOfExit { get; set; }
 


    }
}
