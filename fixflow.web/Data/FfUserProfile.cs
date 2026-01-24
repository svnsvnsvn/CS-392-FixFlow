using fixflow.web.Data;

namespace fixflow.web.Data
{
    public class FfUserProfile
    {
        public string EmployeeId { get; set; } = string.Empty;       // PK/FK  Users UUID as assigned in AspNetUsers, must remain as string type
        public AppUser? User { get; set; }                           // "Pointer" to AppUser. Fields in this class are related to fields in AppUser
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public AppUser? Manager { get; set; }                        // "Pointer" to AppUser. Fields in this class are related to fields in AppUser
        public string? ManagerId { get; set; }                       // UUID of Manager, must remain as string type as all ASP.NET userId


    }
}