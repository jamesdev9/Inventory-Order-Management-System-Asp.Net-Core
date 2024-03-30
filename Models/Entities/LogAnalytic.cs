using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class LogAnalytic : _Base
    {
        public LogAnalytic() { }

        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? IPAddress { get; set; }
        public string? Url { get; set; }
        public string? Device { get; set; }
        public string? GeographicLocation { get; set; }
        public string? Browser { get; set; }

    }
}
