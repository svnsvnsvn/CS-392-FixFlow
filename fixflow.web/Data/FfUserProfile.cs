using fixflow.web.Data;

namespace fixflow.web.Data
{
    public class FfUserProfile
    {
        public string FfUserId { get; set; } = string.Empty;       // PK/FK  Users UUID as assigned in AspNetUsers, must remain as string type
        public AppUser? FfUser { get; set; }                           // "Pointer" to AppUser. Fields in this class are related to fields in AppUser
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public FfBuildingDirectory? Location { get; set; }
        public int LocationCode { get; set; }
        public int Unit { get; set; }
    }
}