using fixflow.web.Data;

namespace fixflow.web.Dto
{
    public class TicketHistoryItemDto
    {
        public int TicketStatus { get; set; }
        public string Assignee { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
