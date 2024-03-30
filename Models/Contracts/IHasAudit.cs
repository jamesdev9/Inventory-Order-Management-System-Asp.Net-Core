using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.Models.Contracts
{
    public interface IHasAudit
    {
        string? CreatedByUserId { get; set; }
        DateTime? CreatedAtUtc { get; set; }
        string? UpdatedByUserId { get; set; }
        DateTime? UpdatedAtUtc { get; set; }



        //not mapped



        [NotMapped]
        string? CreatedByUserName { get; set; }
        [NotMapped]
        string? UpdatedByUserName { get; set; }
        [NotMapped]
        string? CreatedAtString { get; set; }
        [NotMapped]
        string? UpdatedAtString { get; set; }
    }
}
