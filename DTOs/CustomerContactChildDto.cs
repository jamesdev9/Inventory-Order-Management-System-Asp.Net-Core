namespace Indotalent.DTOs
{
    public class CustomerContactChildDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? JobTitle { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int? CustomerId { get; set; }
        public Guid? RowGuid { get; set; }
    }
}
