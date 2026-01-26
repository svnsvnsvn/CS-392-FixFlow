namespace fixflow.web.Data
{
    public class FfTicketRegister
    {
        public Guid TicketId { get; set; }
        public string TicketShortCode { get; set; } = string.Empty;
        public FfUserProfile? EnteredByUser { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public FfUserProfile? RequestedByUser { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public FfBuildingDirectory? Building { get; set; }
        public int Location { get; set; }
        public int Unit { get; set; }
        public FfTicketTypes? TicketType { get; set; }
        public int TicketTroubleType { get; set; }
        public FfStatusCodes? StatusCode { get; set; }
        public int TicketStatus { get; set; }
        public FfPriorityCodes? PriorityCode { get; set; }
        public int TicketPriority { get; set; }
    }
}
