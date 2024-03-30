namespace Indotalent.DTOs
{
    public class NumberSequenceDto
    {
        public int? Id { get; set; }
        public string? EntityName { get; set; }
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public int? LastUsedCount { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
