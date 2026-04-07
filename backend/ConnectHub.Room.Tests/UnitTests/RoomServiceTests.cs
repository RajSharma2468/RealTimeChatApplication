using Moq;
using NUnit.Framework;
using ConnectHub.Room.Models;
using ConnectHub.Room.DTOs;
using ConnectHub.Room.Services;
using ConnectHub.Room.Repositories;
using ConnectHub.Room.Tests.Helpers;

namespace ConnectHub.Room.Tests.UnitTests
{
    [TestFixture]
    public class RoomServiceTests
    {
        private Mock<IRoomRepository> _mockRepo;
        private RoomService _service;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRoomRepository>();
            _service = new RoomService(_mockRepo.Object);
        }
        
        // Test: Create room successfully
        [Test]
        public async Task CreateRoomAsync_ValidData_ReturnsRoomResponse()
        {
            // Arrange
            var dto = TestDataBuilder.CreateValidCreateRoomDto();
            var roomToCreate = TestDataBuilder.CreateTestRoom(id: 0, roomName: dto.RoomName);
            var createdRoom = TestDataBuilder.CreateTestRoom(id: 10, roomName: dto.RoomName);
            
            _mockRepo.Setup(x => x.CreateRoomAsync(It.IsAny<ChatRoom>())).ReturnsAsync(createdRoom);
            _mockRepo.Setup(x => x.AddMemberAsync(It.IsAny<RoomMember>())).ReturnsAsync(new RoomMember());
            _mockRepo.Setup(x => x.GetMemberCountAsync(10)).ReturnsAsync(1);
            _mockRepo.Setup(x => x.GetMemberAsync(10, 1)).ReturnsAsync(new RoomMember { Role = "ADMIN" });
            
            // Act
            var result = await _service.CreateRoomAsync(1, dto);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoomName, Is.EqualTo(dto.RoomName));
            _mockRepo.Verify(x => x.CreateRoomAsync(It.IsAny<ChatRoom>()), Times.Once);
        }
        
        // Test: Create room with empty name throws exception
        [Test]
        public void CreateRoomAsync_EmptyRoomName_ThrowsException()
        {
            // Arrange
            var dto = new CreateRoomDto { RoomName = "", RoomType = "PUBLIC" };
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.CreateRoomAsync(1, dto));
            Assert.That(ex.Message, Is.EqualTo("Room name is required"));
        }
        
        // Test: Get room by ID returns room
        [Test]
        public async Task GetRoomByIdAsync_ExistingRoom_ReturnsRoom()
        {
            // Arrange
            var room = TestDataBuilder.CreateTestRoom(id: 1, roomName: "Test Room");
            _mockRepo.Setup(x => x.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _mockRepo.Setup(x => x.GetMemberCountAsync(1)).ReturnsAsync(5);
            _mockRepo.Setup(x => x.GetMemberAsync(1, 1)).ReturnsAsync(new RoomMember { Role = "ADMIN" });
            
            // Act
            var result = await _service.GetRoomByIdAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.UserRole, Is.EqualTo("ADMIN"));
        }
        
        // Test: Get non-existing room throws exception
        [Test]
        public void GetRoomByIdAsync_NonExistingRoom_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetRoomByIdAsync(999)).ReturnsAsync((ChatRoom)null);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.GetRoomByIdAsync(999, 1));
            Assert.That(ex.Message, Is.EqualTo("Room not found"));
        }
        
        // Test: Join public room successfully
        [Test]
        public async Task JoinRoomAsync_PublicRoom_AddsMember()
        {
            // Arrange
            var room = TestDataBuilder.CreateTestRoom(id: 1, roomType: "PUBLIC");
            _mockRepo.Setup(x => x.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _mockRepo.Setup(x => x.IsUserInRoomAsync(1, 5)).ReturnsAsync(false);
            _mockRepo.Setup(x => x.GetMemberCountAsync(1)).ReturnsAsync(10);
            _mockRepo.Setup(x => x.AddMemberAsync(It.IsAny<RoomMember>())).ReturnsAsync(new RoomMember());
            
            // Act
            var result = await _service.JoinRoomAsync(1, 5);
            
            // Assert
            Assert.That(result, Is.True);
            _mockRepo.Verify(x => x.AddMemberAsync(It.IsAny<RoomMember>()), Times.Once);
        }
        
        // Test: Join private room throws exception
        [Test]
        public void JoinRoomAsync_PrivateRoom_ThrowsException()
        {
            // Arrange
            var room = TestDataBuilder.CreateTestRoom(id: 1, roomType: "PRIVATE");
            _mockRepo.Setup(x => x.GetRoomByIdAsync(1)).ReturnsAsync(room);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.JoinRoomAsync(1, 5));
            Assert.That(ex.Message, Is.EqualTo("Cannot join private room"));
        }
        
        // Test: Join room when already member throws exception
        [Test]
        public void JoinRoomAsync_AlreadyMember_ThrowsException()
        {
            // Arrange
            var room = TestDataBuilder.CreateTestRoom(id: 1, roomType: "PUBLIC");
            _mockRepo.Setup(x => x.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _mockRepo.Setup(x => x.IsUserInRoomAsync(1, 1)).ReturnsAsync(true);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.JoinRoomAsync(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Already a member of this room"));
        }
        
        // Test: Leave room successfully
        [Test]
        public async Task LeaveRoomAsync_ValidMember_RemovesMember()
        {
            // Arrange
            _mockRepo.Setup(x => x.IsUserInRoomAsync(1, 1)).ReturnsAsync(true);
            _mockRepo.Setup(x => x.GetMemberAsync(1, 1)).ReturnsAsync(new RoomMember { Role = "MEMBER" });
            _mockRepo.Setup(x => x.RemoveMemberAsync(1, 1)).ReturnsAsync(true);
            
            // Act
            var result = await _service.LeaveRoomAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.True);
            _mockRepo.Verify(x => x.RemoveMemberAsync(1, 1), Times.Once);
        }
        
        // Test: Leave room when not member throws exception
        [Test]
        public void LeaveRoomAsync_NotMember_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(x => x.IsUserInRoomAsync(1, 5)).ReturnsAsync(false);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.LeaveRoomAsync(1, 5));
            Assert.That(ex.Message, Is.EqualTo("You are not a member of this room"));
        }
        
        // Test: Update member role as admin
        [Test]
        public async Task UpdateMemberRoleAsync_AsAdmin_UpdatesRole()
        {
            // Arrange
            var dto = TestDataBuilder.CreateUpdateRoleDto(roomId: 1, userId: 2, newRole: "ADMIN");
            _mockRepo.Setup(x => x.GetMemberAsync(1, 1)).ReturnsAsync(new RoomMember { Role = "ADMIN" });
            _mockRepo.Setup(x => x.GetMemberAsync(1, 2)).ReturnsAsync(new RoomMember { Role = "MEMBER" });
            _mockRepo.Setup(x => x.UpdateMemberRoleAsync(1, 2, "ADMIN")).ReturnsAsync(true);
            
            // Act
            var result = await _service.UpdateMemberRoleAsync(1, dto);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        // Test: Update member role as non-admin throws exception
        [Test]
        public void UpdateMemberRoleAsync_NotAdmin_ThrowsException()
        {
            // Arrange
            var dto = TestDataBuilder.CreateUpdateRoleDto();
            _mockRepo.Setup(x => x.GetMemberAsync(1, 5)).ReturnsAsync(new RoomMember { Role = "MEMBER" });
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.UpdateMemberRoleAsync(5, dto));
            Assert.That(ex.Message, Is.EqualTo("Only room admin can update member roles"));
        }
        
        // Test: IsUserAdmin returns true for admin
        [Test]
        public async Task IsUserAdminAsync_AdminUser_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetMemberAsync(1, 1)).ReturnsAsync(new RoomMember { Role = "ADMIN" });
            
            // Act
            var result = await _service.IsUserAdminAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        // Test: IsUserAdmin returns false for regular member
        [Test]
        public async Task IsUserAdminAsync_RegularUser_ReturnsFalse()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetMemberAsync(1, 2)).ReturnsAsync(new RoomMember { Role = "MEMBER" });
            
            // Act
            var result = await _service.IsUserAdminAsync(1, 2);
            
            // Assert
            Assert.That(result, Is.False);
        }
    }
}