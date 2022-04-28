using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.Indentity
{
    public class User : IdentityUser<int>
    {
      public int? ClassId { get; set; }
      public bool InSchool { get; set; }

    }
}
