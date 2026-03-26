namespace fixflow.web.Dto
{
    public class UserSettingsListItemDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int LocationCode { get; set; }
        public int Unit { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool ResetPassOnLogin { get; set; }
    }
}
