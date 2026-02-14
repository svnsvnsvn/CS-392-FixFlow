namespace fixflow.web.Data
{
    public class FfTicketFlow
    {
        public long ActionId { get; set; }
        public FfTicketRegister? Ticket {  get; set; }
        public Guid TicketId { get; set; }
        public FfStatusCodes? StatusCode { get; set; }
        public int NewTicketStatus { get; set; }
        public FfUserProfile? User {  get; set; }
        public String NewAssignee { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    }
}
