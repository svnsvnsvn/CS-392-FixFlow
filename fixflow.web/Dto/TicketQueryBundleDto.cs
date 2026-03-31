using fixflow.web.Data;

namespace fixflow.web.Dto
{
    public class TicketQueryBundleDto
    {
        public List<FfTicketRegister> Tickets { get; set; } = new();
        public List<FfTicketFlow> Flows { get; set; } = new();
        public Dictionary<string, string> ProfileDisplayNames { get; set; } = new();
        public Dictionary<int, string> StatusCodeNames { get; set; } = new();
    }
}
