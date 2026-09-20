using Microsoft.AspNetCore.Identity;

namespace SchoolPortal.Identities.Domain.Users;

public class ApplicationUser : IdentityUser<int>
{
    public string Name { get; set; } = null!;
    
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    public DateTime? RestoredAt { get; set; }
    public int? RestoredByUserId { get; set; }
}