using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class Customer : _Base
    {
        public Customer() { }
        public required string Name { get; set; }
        public string? Number { get; set; }
        public string? Description { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Website { get; set; }
        public string? WhatsApp { get; set; }
        public string? LinkedIn { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? TwitterX { get; set; }
        public string? TikTok { get; set; }
        public required int CustomerGroupId { get; set; }
        public CustomerGroup? CustomerGroup { get; set; }
        public required int CustomerCategoryId { get; set; }
        public CustomerCategory? CustomerCategory { get; set; }
    }
}
