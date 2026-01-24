using Microsoft.AspNetCore.Identity;

namespace fixflow.web.Data;

public class AppUser : IdentityUser
{
    // Optional extra fields
    public enum LoginMode { LocalOnly, OidcOnly, LocalAndOidc};         // Login modes authorized
    public bool PassswordChangeRequired { get; set; } = false;          // Account locked for password change, not hard locked
    public DateTime PasswordExpire {  get; set; } = DateTime.Now + TimeSpan.FromDays(60);
}
