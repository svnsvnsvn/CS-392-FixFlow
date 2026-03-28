using fixflow.web.Data;

namespace fixflow.web.Dto
{
    public class NewTicketDto
    {
        public string? RequestedBy { get; set; }
        public int Location { get; set; }
        public int Unit { get; set; }
        public int TicketTroubleType { get; set; }
        public int TicketStatus { get; set; }
        public int TicketPriority { get; set; }
        public string TicketSubject { get; set; } = string.Empty;
        public string TicketDescription { get; set; } = string.Empty;
    }
}
