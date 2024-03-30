namespace Indotalent.DTOs
{
    public class LogSessionDto
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? IPAddress { get; set; }
        public string? Action { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
