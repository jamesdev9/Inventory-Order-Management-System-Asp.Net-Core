namespace Indotalent.DTOs
{
    public class LogAnalyticDto
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? IPAddress { get; set; }
        public string? Url { get; set; }
        public string? Device { get; set; }
        public string? GeographicLocation { get; set; }
        public string? Browser { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
