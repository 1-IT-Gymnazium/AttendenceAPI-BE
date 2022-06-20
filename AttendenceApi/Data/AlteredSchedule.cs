using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class AlteredSchedule
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public int? StartOfLessonsInMinutes { get; set; }
        public int? EndTimeOfLessonsInMinutes { get; set; }

        [Column(TypeName ="date")]
        public DateTime Date { get; set; }
   

    }
}
