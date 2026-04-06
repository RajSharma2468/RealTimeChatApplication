namespace ConnectHub.Room.Tests.Helpers
{
    // Centralized test constants - keeps all magic strings/numbers in one place
    public static class TestConstants
    {
        // ========== ROOM CONSTANTS ==========
        public const int ExistingRoomId = 1;
        public const int NonExistingRoomId = 999;
        public const int PublicRoomId = 1;
        public const int PrivateRoomId = 3;
        
        // ========== USER CONSTANTS ==========
        public const int AdminUserId = 1;
        public const int RegularUserId = 2;
        public const int NonMemberUserId = 5;
        
        // ========== ROOM NAMES ==========
        public const string RoomNameCricket = "Cricket Fans";
        public const string RoomNameFootball = "Football Fans";
        public const string NewRoomName = "New Test Room";
        
        // ========== ROLES ==========
        public const string RoleAdmin = "ADMIN";
        public const string RoleMember = "MEMBER";
        
        // ========== ROOM TYPES ==========
        public const string RoomTypePublic = "PUBLIC";
        public const string RoomTypePrivate = "PRIVATE";
        
        // ========== ERROR MESSAGES ==========
        public const string ErrorRoomNotFound = "Room not found";
        public const string ErrorAlreadyMember = "Already a member of this room";
        public const string ErrorRoomFull = "Room is full";
        public const string ErrorNotAdmin = "Only room admin can update member roles";
        public const string ErrorCannotJoinPrivate = "Cannot join private room";
        public const string ErrorRoomNameRequired = "Room name is required";
        
        // ========== PAGINATION ==========
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 10;
        public const int MaxMembersDefault = 500;
    }
}