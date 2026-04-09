namespace ConnectHub.Admin.Tests.Helpers
{
    public static class TestConstants
    {
        // User IDs
        public const int AdminUserId = 1;
        public const int TargetUserId = 2;
        public const int NonExistingUserId = 999;
        
        // Room IDs
        public const int ExistingRoomId = 1;
        public const int NonExistingRoomId = 999;
        
        // Message IDs
        public const int ExistingMessageId = 1;
        public const int NonExistingMessageId = 999;
        
        // Action types
        public const string ActionSuspendUser = "SUSPEND_USER";
        public const string ActionDeleteUser = "DELETE_USER";
        public const string ActionDeleteRoom = "DELETE_ROOM";
        public const string ActionDeleteMessage = "DELETE_MESSAGE";
        
        // Error messages
        public const string ErrorUserNotFound = "User not found";
        public const string ErrorRoomNotFound = "Room not found";
        public const string ErrorMessageNotFound = "Message not found";
        public const string ErrorUnauthorized = "Unauthorized access";
        public const string ErrorForbidden = "Forbidden - Admin access required";
        
        // Pagination
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 50;
    }
}