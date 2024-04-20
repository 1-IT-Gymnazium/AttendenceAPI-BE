

using AttendenceApi.Data.Indentity;

namespace AttendenceApi.Data;

public class Claims
{
    public const string SUPERUSER = User.Entry.ClaimTypeSuperUser;
    public const string USER = "CLAIM_USER";
    public const string TEACHER = "CLAIM_TEACHER";
    public const string PARENT = "CLAIM_PARENT";

}
