namespace ConnectHub.Messaging.Tests.Helpers
{
    /// Centralized test constants for messaging
    public static class TestConstants
    {
        // ========== USER IDS ==========
        public const int User1Id = 1;
        public const int User2Id = 2;
        public const int User3Id = 3;
        public const int NonExistingUser = 999;
        
        // ========== MESSAGE IDS ==========
        public const int ExistingMessageId = 1;
        public const int NonExistingMessageId = 999;
        
        // ========== ROOM IDS ==========
        public const int RoomId = 1;
        public const int NonExistingRoom = 999;
        
        // ========== CONTENT ==========
        public const string TestMessage = "Hello, this is a test message";
        public const string UpdatedMessage = "This message has been edited";
        public const string SearchKeyword = "Hello";
        public const string EmptySearchKeyword = "";
        
        // ========== PAGINATION ==========
        public const int Page1 = 1;
        public const int Page2 = 2;
        public const int PageSize10 = 10;
        public const int PageSize20 = 20;
        
        // ========== ERROR MESSAGES ==========
        public const string ErrorMessageNotFound = "Message not found";
        public const string ErrorCannotEditOwn = "You can only edit your own messages";
        public const string ErrorCannotDeleteOwn = "You can only delete your own messages";
        public const string ErrorCannotEditDeleted = "Cannot edit deleted message";
    }
}