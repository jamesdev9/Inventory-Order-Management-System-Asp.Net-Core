using Indotalent.Models.Contracts;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.Models.Entities
{
    public class ApplicationUser : IdentityUser, IHasAudit, IHasSoftDelete
    {
        public ApplicationUser()
        {
            this.CreatedAtUtc = DateTime.UtcNow;
        }
        public string? FullName { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? Avatar { get; set; }
        public UserType UserType { get; set; } = UserType.Internal;
        public bool IsDefaultAdmin { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public required int SelectedCompanyId { get; set; }
        public Company? SelectedCompany { get; set; }


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
        public bool IsNotDeleted { get; set; } = true;
    }
}
