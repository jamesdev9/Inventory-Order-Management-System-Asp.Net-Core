namespace Indotalent.DTOs
{
    public class UserProfileDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? Avatar { get; set; }
        public string? UserType { get; set; }
        public bool IsDefaultAdmin { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public string? SelectedCompany { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
