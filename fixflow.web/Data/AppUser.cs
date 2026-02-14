using Microsoft.AspNetCore.Identity;

namespace fixflow.web.Data;

public class AppUser : IdentityUser
{
    // Optional extra fields
    public bool ResetPassOnLogin { get; set; } = false;          // Account locked for password change, not hard locked
    public DateTime PasswordExpire {  get; set; } = DateTime.Now + TimeSpan.FromDays(60);
}
