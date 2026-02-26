namespace fixflow.web.Dto
{
    public class UpdateTicketDto
    {
        public Guid TicketId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public int Location { get; set; }
        public int Unit { get; set; }
        public int TicketTroubleType { get; set; }
        public int TicketPriority { get; set; }
        public string TicketSubject { get; set; } = string.Empty;
        public string TicketDescription { get; set; } = string.Empty;
    }
}
