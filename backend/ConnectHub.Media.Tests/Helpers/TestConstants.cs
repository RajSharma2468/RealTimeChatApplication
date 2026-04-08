namespace ConnectHub.Media.Tests.Helpers
{
    // Centralized test constants
    public static class TestConstants
    {
        // User IDs
        public const int USER_ID_1 = 1;
        public const int USER_ID_2 = 2;
        public const int NON_EXISTING_USER_ID = 999;
        
        // File IDs
        public const int EXISTING_FILE_ID = 1;
        public const int NON_EXISTING_FILE_ID = 999;
        
        // Message IDs
        public const int MESSAGE_ID = 100;
        public const int ROOM_ID = 200;
        
        // File limits
        public const int MAX_FILE_SIZE_BYTES = 10 * 1024 * 1024; // 10MB
        public const int SMALL_FILE_SIZE_BYTES = 1024; // 1KB
        
        // Error messages
        public const string ERROR_FILE_NOT_FOUND = "File not found";
        public const string ERROR_NO_FILE = "No file provided";
        public const string ERROR_FILE_TOO_LARGE = "File size exceeds 10MB limit";
        public const string ERROR_NOT_OWNER = "You can only delete your own files";
    }
}