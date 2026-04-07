using ConnectHub.Room.Models;
using ConnectHub.Room.DTOs;

namespace ConnectHub.Room.Tests.Helpers
{
    // TestDataBuilder - Creates fake test data for unit tests
    // Instead of writing new ChatRoom { } every time, just call these methods
    // This keeps your tests clean and maintains test data in one place
    public static class TestDataBuilder
    {
        // ========== ROOM BUILDERS ==========

        // Creates a single test room with default values
        // Most tests just need a basic room - use this with default params
        // Override specific values when you need custom data for edge cases
        // 
        // Example: CreateTestRoom(id: 5, roomName: "Cricket Fans", roomType: "PRIVATE")
        // Example: CreateTestRoom() - returns room with defaults (Id=1, Name="Test Room")
        public static ChatRoom CreateTestRoom(
            int id = 1,
            string roomName = "Test Room",
            string roomType = "PUBLIC",
            int createdBy = 1,
            int maxMembers = 100)
        {
            return new ChatRoom
            {
                Id = id,
                RoomName = roomName,      // Property name is RoomName, not Name
                RoomType = roomType,      // Property name is RoomType, not Type
                CreatedBy = createdBy,
                MaxMembers = maxMembers,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Description = null,
                AvatarUrl = null
            };
        }

        // Creates multiple test rooms in one go
        // Useful when testing lists, pagination, search results
        // Even numbered rooms become PRIVATE, odd become PUBLIC - gives variety for filtering tests
        // 
        // Example: var rooms = CreateTestRooms(5); - creates 5 rooms with IDs 1-5
        public static List<ChatRoom> CreateTestRooms(int count = 3)
        {
            var rooms = new List<ChatRoom>();
            for (int i = 1; i <= count; i++)
            {
                rooms.Add(CreateTestRoom(
                    id: i,
                    roomName: $"Test Room {i}",
                    roomType: i % 2 == 0 ? "PRIVATE" : "PUBLIC",  // Mix of public and private for testing
                    createdBy: 1
                ));
            }
            return rooms;
        }

        // Creates a room with a specific creator (not just default user 1)
        // Use this when testing permissions - admin vs regular member scenarios
        // 
        // Example: var adminRoom = CreateRoomWithCreator(roomId: 10, creatorId: 5);
        public static ChatRoom CreateRoomWithCreator(
            int roomId = 1,
            string roomName = "Admin Room",
            string roomType = "PUBLIC",
            int creatorId = 1)
        {
            return new ChatRoom
            {
                Id = roomId,
                RoomName = roomName,
                RoomType = roomType,
                CreatedBy = creatorId,
                MaxMembers = 100,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        // Creates a room that is full (member count equals max members)
        // Use this to test "room is full" error scenarios
        // 
        // Example: var fullRoom = CreateFullRoom(roomId: 1, maxMembers: 5);
        public static ChatRoom CreateFullRoom(int roomId = 1, int maxMembers = 5)
        {
            return new ChatRoom
            {
                Id = roomId,
                RoomName = "Full Room",
                RoomType = "PUBLIC",
                CreatedBy = 1,
                MaxMembers = maxMembers,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        // ========== ROOM MEMBER BUILDERS ==========

        // Creates a test room member (links a user to a room)
        // Role can be "ADMIN" (can manage members) or "MEMBER" (regular user)
        // Admin can: add/remove members, update roles, delete the room
        // 
        // Example: var member = CreateTestRoomMember(roomId: 1, userId: 5, role: "ADMIN");
        public static RoomMember CreateTestRoomMember(
            int roomId = 1,
            int userId = 1,
            string role = "MEMBER")
        {
            return new RoomMember
            {
                RoomId = roomId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        // Creates multiple members for a room at once
        // Useful when testing member lists, admin permissions, or leaving rooms
        // 
        // Example: var members = CreateRoomMembers(roomId: 1, count: 10);
        public static List<RoomMember> CreateRoomMembers(int roomId = 1, int count = 5)
        {
            var members = new List<RoomMember>();
            for (int i = 1; i <= count; i++)
            {
                members.Add(CreateTestRoomMember(
                    roomId: roomId,
                    userId: i,
                    role: i == 1 ? "ADMIN" : "MEMBER"  // First user is admin
                ));
            }
            return members;
        }

        // Creates a room WITH its members in one shot
        // Returns both the room and the list of members as a tuple
        // Perfect for testing room details page that shows member list
        // 
        // Example: 
        //   var (room, members) = CreateTestRoomWithMembers(roomId: 1, adminId: 1, memberCount: 10);
        //   Assert.That(room.MemberCount, Is.EqualTo(members.Count));
        public static (ChatRoom room, List<RoomMember> members) CreateTestRoomWithMembers(
            int roomId = 1,
            int adminId = 1,
            int memberCount = 5)
        {
            var room = CreateTestRoom(id: roomId, createdBy: adminId);
            var members = new List<RoomMember>();
            
            // First member is the ADMIN (room creator)
            members.Add(CreateTestRoomMember(roomId, adminId, "ADMIN"));
            
            // Rest are regular MEMBERS
            for (int i = 2; i <= memberCount; i++)
            {
                members.Add(CreateTestRoomMember(roomId, i, "MEMBER"));
            }
            
            return (room, members);
        }

        // ========== DTO BUILDERS FOR CONTROLLER TESTS ==========

        // Quick helper for valid CreateRoomDto
        // Use in controller tests where you need valid input that passes validation
        // 
        // Example: var dto = CreateValidCreateRoomDto(roomName: "New Room");
        public static CreateRoomDto CreateValidCreateRoomDto(
            string roomName = "Test Room",
            string roomType = "PUBLIC")
        {
            return new CreateRoomDto
            {
                RoomName = roomName,
                RoomType = roomType
            };
        }

        // Creates invalid DTO with empty room name
        // Use to test validation - should trigger "Room name is required" error
        // 
        // Example: var invalidDto = CreateInvalidCreateRoomDto();
        public static CreateRoomDto CreateInvalidCreateRoomDto()
        {
            return new CreateRoomDto
            {
                RoomName = "",  // Empty - triggers validation error
                RoomType = "PUBLIC"
            };
        }

        // Creates DTO for updating member roles
        // Use when testing admin permissions to promote/demote members
        // 
        // Example: var dto = CreateUpdateRoleDto(roomId: 1, userId: 2, newRole: "ADMIN");
        public static UpdateMemberRoleDto CreateUpdateRoleDto(
            int roomId = 1,
            int userId = 2,
            string newRole = "ADMIN")
        {
            return new UpdateMemberRoleDto
            {
                RoomId = roomId,
                UserId = userId,
                NewRole = newRole
            };
        }

        // ========== HELPER METHODS FOR SPECIFIC TEST SCENARIOS ==========

        // Creates two users with a shared room for messaging tests
        // Returns room, user1 member, user2 member - perfect for chat tests
        // 
        // Example: 
        //   var (room, user1, user2) = CreateRoomWithTwoMembers();
        //   // Test user1 sending message to user2 in the room
        public static (ChatRoom room, RoomMember member1, RoomMember member2) CreateRoomWithTwoMembers(
            int roomId = 1,
            int user1Id = 1,
            int user2Id = 2)
        {
            var room = CreateTestRoom(id: roomId, createdBy: user1Id);
            var member1 = CreateTestRoomMember(roomId, user1Id, "ADMIN");
            var member2 = CreateTestRoomMember(roomId, user2Id, "MEMBER");
            
            return (room, member1, member2);
        }

        // Creates a deleted/inactive room (soft deleted)
        // Use to test that deleted rooms don't appear in lists or searches
        // 
        // Example: var deletedRoom = CreateDeletedRoom(roomId: 99);
        public static ChatRoom CreateDeletedRoom(int roomId = 1)
        {
            return new ChatRoom
            {
                Id = roomId,
                RoomName = "Deleted Room",
                RoomType = "PUBLIC",
                CreatedBy = 1,
                IsActive = false,  // Soft deleted
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
        }

        // Creates a private room (only invited users can join)
        // Use to test that regular users cannot join without invitation
        // 
        // Example: var privateRoom = CreatePrivateRoom(roomId: 5, creatorId: 10);
        public static ChatRoom CreatePrivateRoom(int roomId = 1, int creatorId = 1)
        {
            return new ChatRoom
            {
                Id = roomId,
                RoomName = "Private Room",
                RoomType = "PRIVATE",  // Only invited users can join
                CreatedBy = creatorId,
                MaxMembers = 50,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}