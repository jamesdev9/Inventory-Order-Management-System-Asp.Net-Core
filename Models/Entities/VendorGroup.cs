using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class VendorGroup : _Base
    {
        public VendorGroup() { }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
