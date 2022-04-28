using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class Schedule
    {
        public int Id { get; set; }
        public int? ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public string Day { get; set; } = null!;
        
        public int? EndTimeOfLessonsInMinutes { get; set; }
       
    }
}
