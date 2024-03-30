using Indotalent.Models.Contracts;

namespace Indotalent.Models.Entities
{
    public class CustomerGroup : _Base
    {
        public CustomerGroup() { }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
