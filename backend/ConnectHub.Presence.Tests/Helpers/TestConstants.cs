namespace ConnectHub.Presence.Tests.Helpers
{
    // Centralized test constants - keeps all test data in one place
    public static class TestConstants
    {
        // User Ids
        public const int UserId1 = 1;
        public const int UserId2 = 2;
        public const int UserId3 = 3;
        public const int NonExistingUserId = 999;
        
        // Connection Ids
        public const string ConnectionId1 = "conn_001";
        public const string ConnectionId2 = "conn_002";
        public const string ConnectionId3 = "conn_003";
        
        // Heartbeat settings
        public const int HeartbeatTimeoutSeconds = 60;
        public const int HeartbeatIntervalSeconds = 30;
        
        // Conversation types
        public const string ConversationTypeDirect = "DIRECT";
        public const string ConversationTypeRoom = "ROOM";
        
        // Room Id
        public const int RoomId = 1;
        
        // Message Id
        public const int MessageId = 100;
    }
}