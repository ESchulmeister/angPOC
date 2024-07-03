namespace angPOC.Services
{
    public class Constants
    {
        /// <summary>
        /// SQL Errors -  THROW statements in stored procedures
        /// </summary>
        public static class ErrorCode
        {

            public const int InvalidApplication = 500030;
            public const int InvalidAccount = 500021;
            public const int WrongPassword = 500022;
            public const int NoApplicationPermissions = 500023;
            public const int InvalidUser = 5000024;
            public const int InvaliPermission = 500031;

        }

        public static class Keys
        {
            public static class Session
            {
                public const string UserToken = "utk";
            }
            public static class Context
            {
                public const string User = "User";
            }

        }

        public static class Claims
        {
            public const string ID = "id";
            public const string ApplicationID = "AppID";
            public const string ApplicationValue = "2";
        }

        public static class AntiForgery
        {
            public const string Header = "X-XSRF-TOKEN";
            public const string Cookie = "XSRF-TOKEN";
        }
    }
}
