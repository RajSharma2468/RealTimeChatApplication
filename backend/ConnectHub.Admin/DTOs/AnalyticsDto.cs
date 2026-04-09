namespace ConnectHub.Admin.DTOs
{
    // Analytics data transfer object
    public class AnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers24h { get; set; }
        public int TotalMessages { get; set; }
        public int MessagesToday { get; set; }
        public int TotalRooms { get; set; }
        public int ActiveConnections { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}