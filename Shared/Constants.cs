﻿namespace Shared
{
    public class Constants
    {
        public const string MOBILE = "DesktopHome";
        public const string NON_MOBILE = "MobileHome";
        public const string TOKEN_VALID = "TokenValid";
        public const string CATEGORY = "Category";

        public const string ENVIRONMENT = "Environment";
        public const string ENVIRONMENT_TEST = "Test";
        public const string ENVIRONMENT_PRODUCTION = "Production";
        public const string DB_PROD = "BucketListDbConnStrProd";
        public const string DB_TEST = "BucketListDbConnStrDev";

        public const int REGISTRATION_VALUE_LENGTH = 8;
        public const string EMAIL_AT_SIGN = "@";
    }
    public class BucketListConstants
    {
        public const string DB_CONN = "BucketListDbConnStr";
    }
    //Exist in two places (Fix)
    public class Error
    {
        public const string ERR_000001 = "ERR_000001";
        public const string ERR_000002 = "ERR_000002";
    }
    //Exist in two places (Fix)
    public class ErrorMsg
    {
        public const string ERR_MSG_000001 = "No httpcontext object";
        public const string ERR_MSG_000002 = "Token Expired";
    }
}
