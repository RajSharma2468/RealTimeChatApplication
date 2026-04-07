using NUnit.Framework;
using ConnectHub.Room.Repositories;
using ConnectHub.Room.Tests.Mocks;
using ConnectHub.Room.Models;

namespace ConnectHub.Room.Tests.UnitTests
{
    [TestFixture]
    public class RoomRepositoryTests
    {
        // Test: Create room should add to database
        [Test]
        public async Task CreateRoomAsync_ValidRoom_AddsToDatabase()
        {
            // Arrange: Create in-memory database
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            var newRoom = new ChatRoom
            {
                RoomName = "Brand New Room",
                RoomType = "PUBLIC",
                CreatedBy = 5,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            // Act: Create room
            var result = await repo.CreateRoomAsync(newRoom);
            
            // Assert: Verify room was added
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.RoomName, Is.EqualTo("Brand New Room"));
        }
        
        // Test: Get room by existing ID should return room
        [Test]
        public async Task GetRoomByIdAsync_ExistingId_ReturnsRoom()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Get room with ID 1
            var result = await repo.GetRoomByIdAsync(1);
            
            // Assert: Should find the room
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.RoomName, Is.EqualTo("Cricket Fans"));
        }
        
        // Test: Get room by non-existing ID should return null
        [Test]
        public async Task GetRoomByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Try to get room with ID 999
            var result = await repo.GetRoomByIdAsync(999);
            
            // Assert: Should return null
            Assert.That(result, Is.Null);
        }
        
        // Test: Get public rooms should return only public rooms
        [Test]
        public async Task GetPublicRoomsAsync_ReturnsOnlyPublicRooms()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Get all public rooms
            var result = await repo.GetPublicRoomsAsync();
            
            // Assert: Should return only public rooms (not private)
            Assert.That(result, Is.Not.Null);
            Assert.That(result.All(r => r.RoomType == "PUBLIC"), Is.True);
        }
        
        // Test: Get rooms by user ID should return rooms user joined
        [Test]
        public async Task GetRoomsByUserIdAsync_ExistingUser_ReturnsRooms()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Get rooms for user 1 (admin of room 1 and 3)
            var result = await repo.GetRoomsByUserIdAsync(1);
            
            // Assert: Should return rooms where user is member
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(r => r.Id == 1), Is.True);
        }
        
        // Test: Add member to room
        [Test]
        public async Task AddMemberAsync_ValidMember_AddsToDatabase()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            var newMember = new RoomMember
            {
                RoomId = 1,
                UserId = 10,
                Role = "MEMBER",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            // Act: Add member
            var result = await repo.AddMemberAsync(newMember);
            
            // Assert: Verify member was added
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.UserId, Is.EqualTo(10));
        }
        
        // Test: Get member by room ID and user ID
        [Test]
        public async Task GetMemberAsync_ExistingMember_ReturnsMember()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Get member in room 1 for user 1
            var result = await repo.GetMemberAsync(1, 1);
            
            // Assert: Should find the member
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Role, Is.EqualTo("ADMIN"));
        }
        
        // Test: Check if user is in room
        [Test]
        public async Task IsUserInRoomAsync_UserInRoom_ReturnsTrue()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Check if user 1 is in room 1
            var result = await repo.IsUserInRoomAsync(1, 1);
            
            // Assert: Should return true
            Assert.That(result, Is.True);
        }
        
        // Test: Check if user is not in room
        [Test]
        public async Task IsUserInRoomAsync_UserNotInRoom_ReturnsFalse()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Check if user 5 (non-member) is in room 1
            var result = await repo.IsUserInRoomAsync(1, 5);
            
            // Assert: Should return false
            Assert.That(result, Is.False);
        }
        
        // Test: Get member count for room
        [Test]
        public async Task GetMemberCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Get member count for room 1
            var result = await repo.GetMemberCountAsync(1);
            
            // Assert: Room 1 has 2 members (user 1 and user 2)
            Assert.That(result, Is.EqualTo(2));
        }
        
        // Test: Update member role
        [Test]
        public async Task UpdateMemberRoleAsync_ValidUpdate_ReturnsTrue()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Update user 2 role to ADMIN in room 1
            var result = await repo.UpdateMemberRoleAsync(1, 2, "ADMIN");
            
            // Assert: Should return true
            Assert.That(result, Is.True);
            
            // Verify role was updated
            var member = await repo.GetMemberAsync(1, 2);
            Assert.That(member.Role, Is.EqualTo("ADMIN"));
        }
        
        // Test: Remove member from room (soft delete)
        [Test]
        public async Task RemoveMemberAsync_ValidMember_RemovesFromRoom()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Remove user 2 from room 1
            var result = await repo.RemoveMemberAsync(1, 2);
            
            // Assert: Should return true
            Assert.That(result, Is.True);
            
            // Verify member is no longer active
            var member = await repo.GetMemberAsync(1, 2);
            Assert.That(member, Is.Null);
        }
        
        // Test: Delete room (soft delete)
        [Test]
        public async Task DeleteRoomAsync_ValidRoom_SoftDeletes()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new RoomRepository(context);
            
            // Act: Delete room 1
            var result = await repo.DeleteRoomAsync(1);
            
            // Assert: Should return true
            Assert.That(result, Is.True);
            
            // Verify room is no longer active (filter hides it)
            var room = await repo.GetRoomByIdAsync(1);
            Assert.That(room, Is.Null);
        }
    }
}