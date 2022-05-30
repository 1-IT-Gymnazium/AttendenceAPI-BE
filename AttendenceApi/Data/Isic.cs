using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AttendenceApi.Data.Indentity;

namespace AttendenceApi.Data
{
    public class Isic
    {
        public Guid Id { get; set; }
        public string IsicId { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
