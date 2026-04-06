using NUnit.Framework;
using ConnectHub.Room.Tests.Mocks;

namespace ConnectHub.Room.Tests.IntegrationTests
{
    [TestFixture]
    public class RoomApiTests
    {
        // Test: Complete room lifecycle (create → join → leave → delete)
        [Test]
        public async Task RoomLifecycle_CreateJoinLeaveDelete_WorksCorrectly()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.RoomRepository(context);
            var service = new Services.RoomService(repo);
            
            // Step 1: Create room
            var createDto = new DTOs.CreateRoomDto 
            { 
                RoomName = "Integration Test Room", 
                RoomType = "PUBLIC" 
            };
            var created = await service.CreateRoomAsync(1, createDto);
            Assert.That(created.RoomName, Is.EqualTo("Integration Test Room"));
            
            // Step 2: Another user joins the room
            var joinResult = await service.JoinRoomAsync(created.Id, 2);
            Assert.That(joinResult, Is.True);
            
            // Step 3: Get room details and verify member count
            var room = await service.GetRoomByIdAsync(created.Id, 1);
            Assert.That(room.MemberCount, Is.EqualTo(2));
            
            // Step 4: User 2 leaves the room
            var leaveResult = await service.LeaveRoomAsync(created.Id, 2);
            Assert.That(leaveResult, Is.True);
            
            // Step 5: Delete room
            var deleteResult = await service.DeleteRoomAsync(created.Id, 1);
            Assert.That(deleteResult, Is.True);
        }
        
        // Test: Get public rooms
        [Test]
        public async Task GetPublicRooms_ReturnsOnlyPublicRooms()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.RoomRepository(context);
            var service = new Services.RoomService(repo);
            
            // Act
            var publicRooms = await service.GetPublicRoomsAsync(1);
            
            Assert.That(publicRooms, Is.Not.Null);
            Assert.That(publicRooms.All(r => r.RoomType == "PUBLIC"), Is.True);
        }
        
        // Test: Update member role
        [Test]
        public async Task UpdateMemberRole_PromotesUserToAdmin()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.RoomRepository(context);
            var service = new Services.RoomService(repo);
            
            // Create a room
            var createDto = new DTOs.CreateRoomDto { RoomName = "Role Test Room", RoomType = "PUBLIC" };
            var room = await service.CreateRoomAsync(1, createDto);
            
            // Another user joins
            await service.JoinRoomAsync(room.Id, 2);
            
            // Update role
            var updateDto = new DTOs.UpdateMemberRoleDto 
            { 
                RoomId = room.Id, 
                UserId = 2, 
                NewRole = "ADMIN" 
            };
            var result = await service.UpdateMemberRoleAsync(1, updateDto);
            
            Assert.That(result, Is.True);
            
            // Verify user is now admin
            var isAdmin = await service.IsUserAdminAsync(room.Id, 2);
            Assert.That(isAdmin, Is.True);
        }
        
        // Test: Cannot join private room
        [Test]
        public void JoinPrivateRoom_ThrowsException()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.RoomRepository(context);
            var service = new Services.RoomService(repo);
            
            // Act & Assert - Room 3 is private
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await service.JoinRoomAsync(3, 5));
            Assert.That(ex.Message, Is.EqualTo("Cannot join private room"));
        }
        
        // Test: User cannot join same room twice
        [Test]
        public void JoinSameRoomTwice_ThrowsException()
        {
            // Arrange
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.RoomRepository(context);
            var service = new Services.RoomService(repo);
            
            // Act & Assert - User 1 is already in room 1
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await service.JoinRoomAsync(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Already a member of this room"));
        }
    }
}