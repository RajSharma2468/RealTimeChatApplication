using System.Text.Json;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Tests.Helpers
{
    /// Test Extensions - Helper methods for test assertions and conversions
    public static class TestExtensions
    {
        /// Converts any object to JSON string (for debugging)
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }
        
        /// Checks if two users have same data (for assertion)

        public static bool Matches(this User actual, User expected)
        {
            return actual.Id == expected.Id &&
                   actual.Username == expected.Username &&
                   actual.DisplayName == expected.DisplayName &&
                   actual.Email == expected.Email &&
                   actual.IsActive == expected.IsActive;
        }
        
        /// Creates a deep copy of user (for test isolation)

        public static User Clone(this User user)
        {
            return new User
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastSeen = user.LastSeen
            };
        }
        
        /// Prints test summary to console (for debugging)

        public static void PrintTestSummary(string testName, bool passed, string? message = null)
        {
            var status = passed ? "PASSED" : "FAILED";
            Console.WriteLine($"[{status}] {testName}");
            if (message != null)
                Console.WriteLine($"    {message}");
        }
    }
}