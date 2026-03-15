using fixflow.web.Domain.Enums;

namespace fixflow.web.Dto
{
    public sealed class UserCredentialDTO
    {
        public string? UserId { get; set; } = string.Empty;
        public RoleTypes? Role { get; set; }
    }
}
