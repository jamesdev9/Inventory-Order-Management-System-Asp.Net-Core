using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class VendorCategory : _Base
    {
        public VendorCategory() { }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
