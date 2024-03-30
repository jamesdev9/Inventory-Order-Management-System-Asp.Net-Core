namespace Indotalent.DTOs
{
    public class LogErrorDto
    {
        public int? Id { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? AdditionalInfo { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
