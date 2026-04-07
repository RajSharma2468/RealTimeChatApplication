namespace ConnectHub.Presence.Tests.Helpers
{
    // Centralized test constants
    public static class TestConstants
    {
        // User IDs
        public const int UserId1 = 1;
        public const int UserId2 = 2;
        public const int UserId3 = 3;
        public const int NonExistingUserId = 999;
        
        // Connection IDs
        public const string ConnectionId1 = "conn_001";
        public const string ConnectionId2 = "conn_002";
        public const string ConnectionId3 = "conn_003";
        
        // Timeout values (seconds)
        public const int HeartbeatIntervalSeconds = 30;
        public const int HeartbeatTimeoutSeconds = 60;
        
        // Conversation types
        public const string ConversationTypeDirect = "DIRECT";
        public const string ConversationTypeRoom = "ROOM";
        
        // Room ID
        public const int RoomId = 1;
        
        // Message ID
        public const int MessageId = 100;
    }
}