using AttendenceApi.Data.Indentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class Absence
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public bool Excused { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? TimeOfArrival { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? TimeOfExit { get; set; }
        public string? Reason { get; set; } 
 
    }
}
