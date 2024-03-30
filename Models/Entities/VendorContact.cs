using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class VendorContact : _Base
    {
        public VendorContact() { }
        public required string Name { get; set; }
        public string? Number { get; set; }
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Description { get; set; }
        public required int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
    }
}
