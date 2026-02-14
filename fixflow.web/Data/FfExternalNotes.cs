namespace fixflow.web.Data
{
    public class FfExternalNotes
    {
        public long XNoteId { get; set; }
        public FfTicketRegister? Ticket { get; set; }
        public Guid TicketId { get; set; }
        public FfUserProfile? User { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string Content { get; set; } = string.Empty;
    }
}
