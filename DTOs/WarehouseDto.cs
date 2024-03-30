namespace Indotalent.DTOs
{
    public class WarehouseDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public bool SystemWarehouse { get; set; }
    }
}
