using Microsoft.AspNetCore.Identity;

namespace AttendenceApi.Data.Indentity
{
    public class User : IdentityUser<Guid>
    {
      public Guid? ClassId { get; set; }
      public bool InSchool { get; set; }

    }

}
