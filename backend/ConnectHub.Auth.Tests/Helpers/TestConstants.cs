namespace ConnectHub.Auth.Tests.Helpers
{
    /// <summary>
    /// Test Constants - Centralized test data values
    /// Keeps all magic strings/numbers in one place
    /// </summary>
    public static class TestConstants
    {
        // ========== USER CONSTANTS ==========
        public const int EXISTING_USER_ID = 1;
        public const int NON_EXISTING_USER_ID = 999;
        public const int ADMIN_USER_ID = 1;
        public const int INACTIVE_USER_ID = 3;
        
        // ========== USERNAME CONSTANTS ==========
        public const string EXISTING_USERNAME = "john_doe";
        public const string NON_EXISTING_USERNAME = "nonexistent";
        public const string ADMIN_USERNAME = "admin";
        public const string INACTIVE_USERNAME = "inactive_user";
        
        // ========== EMAIL CONSTANTS ==========
        public const string EXISTING_EMAIL = "john@example.com";
        public const string NON_EXISTING_EMAIL = "nonexistent@example.com";
        
        // ========== PASSWORD CONSTANTS ==========
        public const string CORRECT_PASSWORD = "Password123";
        public const string INCORRECT_PASSWORD = "WrongPassword";
        public const string DEFAULT_PASSWORD = "Test@123456";
        
        // ========== JWT CONSTANTS ==========
        public const string JWT_SECRET_KEY = "ThisIsA32ByteLongSecretKeyForTesting!";
        public const string JWT_ISSUER = "ConnectHub";
        public const string JWT_AUDIENCE = "ConnectHubClient";
        
        // ========== SEARCH CONSTANTS ==========
        public const string SEARCH_KEYWORD_JOHN = "john";
        public const string SEARCH_KEYWORD_JANE = "jane";
        public const string EMPTY_SEARCH_KEYWORD = "";
        public const string SHORT_SEARCH_KEYWORD = "a";
        
        // ========== PAGINATION CONSTANTS ==========
        public const int DEFAULT_PAGE = 1;
        public const int DEFAULT_PAGE_SIZE = 10;
        public const int LARGE_PAGE_SIZE = 100;
        
        // ========== MESSAGE CONSTANTS ==========
        public const string SUCCESS_MESSAGE = "success";
        public const string ERROR_MESSAGE_USERNAME_EXISTS = "Username already exists";
        public const string ERROR_MESSAGE_EMAIL_EXISTS = "Email already exists";
        public const string ERROR_MESSAGE_INVALID_CREDENTIALS = "Invalid username or password";
        public const string ERROR_MESSAGE_ACCOUNT_INACTIVE = "Account is deactivated. Contact admin.";
    }
}