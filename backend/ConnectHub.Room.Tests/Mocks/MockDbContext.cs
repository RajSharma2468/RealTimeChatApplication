using Microsoft.EntityFrameworkCore;
using ConnectHub.Room.Data;
using ConnectHub.Room.Models;

namespace ConnectHub.Room.Tests.Mocks
{
    // Mock Database Context - Creates in-memory database for testing
    // No real PostgreSQL needed, tests run faster and isolated
    public static class MockDbContext
    {
        // Returns a fresh in-memory database with seed data
        // Each test gets its own isolated database
        public static RoomDbContext GetDbContext()
        {
            // Create unique database name for each test (prevents test interference)
            var options = new DbContextOptionsBuilder<RoomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new RoomDbContext(options);
            
            // Seed test data
            SeedRooms(context);
            SeedRoomMembers(context);
            
            return context;
        }
        
        // Seed test rooms into database
        private static void SeedRooms(RoomDbContext context)
        {
            var rooms = new List<ChatRoom>
            {
                // Room 1: Public room with 2 members (user 1 as ADMIN, user 2 as MEMBER)
                new ChatRoom 
                { 
                    Id = 1, 
                    RoomName = "Cricket Fans", 
                    Description = "Discuss cricket matches",
                    RoomType = "PUBLIC", 
                    CreatedBy = 1, 
                    CreatedAt = DateTime.UtcNow.AddDays(-5), 
                    IsActive = true, 
                    MaxMembers = 500 
                },
                
                // Room 2: Public room with 1 member (user 2 as ADMIN)
                new ChatRoom 
                { 
                    Id = 2, 
                    RoomName = "Football Fans", 
                    Description = "Discuss football matches",
                    RoomType = "PUBLIC", 
                    CreatedBy = 2, 
                    CreatedAt = DateTime.UtcNow.AddDays(-3), 
                    IsActive = true, 
                    MaxMembers = 500 
                },
                
                // Room 3: Private room (only invited users can join)
                new ChatRoom 
                { 
                    Id = 3, 
                    RoomName = "Private Room", 
                    Description = "Private discussion",
                    RoomType = "PRIVATE", 
                    CreatedBy = 1, 
                    CreatedAt = DateTime.UtcNow.AddDays(-1), 
                    IsActive = true, 
                    MaxMembers = 100 
                }
            };
            
            context.ChatRooms.AddRange(rooms);
            context.SaveChanges();
        }
        
        // Seed test room members into database
        private static void SeedRoomMembers(RoomDbContext context)
        {
            var members = new List<RoomMember>
            {
                // Room 1 members: user 1 (ADMIN), user 2 (MEMBER)
                new RoomMember 
                { 
                    Id = 1, 
                    RoomId = 1, 
                    UserId = 1, 
                    Role = "ADMIN", 
                    JoinedAt = DateTime.UtcNow.AddDays(-5), 
                    IsActive = true 
                },
                new RoomMember 
                { 
                    Id = 2, 
                    RoomId = 1, 
                    UserId = 2, 
                    Role = "MEMBER", 
                    JoinedAt = DateTime.UtcNow.AddDays(-4), 
                    IsActive = true 
                },
                
                // Room 2 members: user 2 (ADMIN)
                new RoomMember 
                { 
                    Id = 3, 
                    RoomId = 2, 
                    UserId = 2, 
                    Role = "ADMIN", 
                    JoinedAt = DateTime.UtcNow.AddDays(-3), 
                    IsActive = true 
                },
                
                // Room 3 members: user 1 (ADMIN)
                new RoomMember 
                { 
                    Id = 4, 
                    RoomId = 3, 
                    UserId = 1, 
                    Role = "ADMIN", 
                    JoinedAt = DateTime.UtcNow.AddDays(-1), 
                    IsActive = true 
                }
            };
            
            context.RoomMembers.AddRange(members);
            context.SaveChanges();
        }
    }
}