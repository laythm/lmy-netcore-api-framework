namespace Common.Enums
{
    public static class RolesEnum
    {
        public const string Admin = "Admin";
        public const string ProjectManager = "ProjectManager";
    }

    public static class ApiMessages
    {
        public const string Success = "api.success";
        public const string Error = "api.error";
        public static string FormatApiMessage(string value)
        {
            return "api." + value;
        }
    }

    public class SortDirection
    {
        public const string asc = "asc";
        public const string desc = "desc";
    }
}
