using AttendenceApi.Data.Indentity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Classes Class { get; set; } = null!;
        public string Day { get; set; } = null!;
        public string Date { get; set; } = null!;

       
        public int? EndTimeOfLessonsInMinutes { get; set; }
       
    }
}
