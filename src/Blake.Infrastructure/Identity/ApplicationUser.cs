using Microsoft.AspNetCore.Identity;

namespace Dynasty.Carrington.Blake.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
}
