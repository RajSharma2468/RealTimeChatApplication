using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using ConnectHub.Room.Controllers;
using ConnectHub.Room.DTOs;
using ConnectHub.Room.Services;
using ConnectHub.Room.Tests.Helpers;

namespace ConnectHub.Room.Tests.UnitTests
{
    [TestFixture]
    public class RoomControllerTests
    {
        private Mock<IRoomService> _mockService;
        private RoomController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IRoomService>();
            _controller = new RoomController(_mockService.Object);
            
            // Mock JWT claims to simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        // Test: Create room returns 200 OK
        [Test]
        public async Task CreateRoom_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = TestDataBuilder.CreateValidCreateRoomDto();
            var response = new RoomResponseDto { Id = 10, RoomName = dto.RoomName };
            
            _mockService.Setup(x => x.CreateRoomAsync(1, dto)).ReturnsAsync(response);
            
            // Act
            var result = await _controller.CreateRoom(dto);
            
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Create room with invalid data returns BadRequest
        [Test]
        public async Task CreateRoom_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var dto = TestDataBuilder.CreateInvalidCreateRoomDto();
            _mockService.Setup(x => x.CreateRoomAsync(1, dto)).ThrowsAsync(new Exception("Room name is required"));
            
            // Act
            var result = await _controller.CreateRoom(dto);
            
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        
        // Test: Get room by ID returns 200 OK
        [Test]
        public async Task GetRoomById_ExistingRoom_ReturnsOk()
        {
            // Arrange
            var response = new RoomResponseDto { Id = 1, RoomName = "Test Room" };
            _mockService.Setup(x => x.GetRoomByIdAsync(1, 1)).ReturnsAsync(response);
            
            // Act
            var result = await _controller.GetRoomById(1);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Get non-existing room returns 404
        [Test]
        public async Task GetRoomById_NonExistingRoom_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(x => x.GetRoomByIdAsync(999, 1)).ThrowsAsync(new Exception("Room not found"));
            
            // Act
            var result = await _controller.GetRoomById(999);
            
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        
        // Test: Get public rooms returns 200 OK
        [Test]
        public async Task GetPublicRooms_ReturnsOk()
        {
            // Arrange
            var rooms = new List<RoomListDto>();
            _mockService.Setup(x => x.GetPublicRoomsAsync(1)).ReturnsAsync(rooms);
            
            // Act
            var result = await _controller.GetPublicRooms();
            
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Get my rooms returns 200 OK
        [Test]
        public async Task GetMyRooms_ReturnsOk()
        {
            // Arrange
            var rooms = new List<RoomListDto>();
            _mockService.Setup(x => x.GetMyRoomsAsync(1)).ReturnsAsync(rooms);
            
            // Act
            var result = await _controller.GetMyRooms();
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Join room returns 200 OK
        [Test]
        public async Task JoinRoom_ValidRequest_ReturnsOk()
        {
            // Arrange
            _mockService.Setup(x => x.JoinRoomAsync(1, 1)).ReturnsAsync(true);
            
            // Act
            var result = await _controller.JoinRoom(1);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Join room with error returns BadRequest
        [Test]
        public async Task JoinRoom_AlreadyMember_ReturnsBadRequest()
        {
            // Arrange
            _mockService.Setup(x => x.JoinRoomAsync(1, 1)).ThrowsAsync(new Exception("Already a member"));
            
            // Act
            var result = await _controller.JoinRoom(1);
            
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        
        // Test: Leave room returns 200 OK
        [Test]
        public async Task LeaveRoom_ValidRequest_ReturnsOk()
        {
            // Arrange
            _mockService.Setup(x => x.LeaveRoomAsync(1, 1)).ReturnsAsync(true);
            
            // Act
            var result = await _controller.LeaveRoom(1);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Update member role returns 200 OK
        [Test]
        public async Task UpdateMemberRole_ValidRequest_ReturnsOk()
        {
            // Arrange
            var dto = TestDataBuilder.CreateUpdateRoleDto();
            _mockService.Setup(x => x.UpdateMemberRoleAsync(1, dto)).ReturnsAsync(true);
            
            // Act
            var result = await _controller.UpdateMemberRole(dto);
            
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Delete room returns 200 OK
        [Test]
        public async Task DeleteRoom_ValidRequest_ReturnsOk()
        {
            // Arrange
            _mockService.Setup(x => x.DeleteRoomAsync(1, 1)).ReturnsAsync(true);
            
            // Act
            var result = await _controller.DeleteRoom(1);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}