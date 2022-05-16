﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceApi.Data
{
    public class AlteredSchedule
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public int? StartOfLessonsInMinutes { get; set; }
        public int? EndTimeOfLessonsInMinutes { get; set; }
        public DateTime Date { get; set; }
   

    }
}