using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.Models.Contracts
{
    public class _Base : IHasId, IHasAudit, IHasSoftDelete
    {

        public _Base()
        {
            this.IsNotDeleted = true;
            this.CreatedAtUtc = DateTime.UtcNow;
        }

        //IHasId
        public int Id { get; set; }
        public Guid RowGuid { get; set; }


        //IHasAudit
        public string? CreatedByUserId { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public string? UpdatedByUserId { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        [NotMapped]
        public string? CreatedByUserName { get; set; }
        [NotMapped]
        public string? UpdatedByUserName { get; set; }
        [NotMapped]
        public string? CreatedAtString { get; set; }
        [NotMapped]
        public string? UpdatedAtString { get; set; }


        //IHasSoftDelete
        public bool IsNotDeleted { get; set; }
    }
}
