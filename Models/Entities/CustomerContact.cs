using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class CustomerContact : _Base
    {
        public CustomerContact() { }
        public required string Name { get; set; }
        public string? Number { get; set; }
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Description { get; set; }
        public required int CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
