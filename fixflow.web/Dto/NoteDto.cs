namespace fixflow.web.Dto
{
    public class NoteDto
    {
        public Guid TicketId { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public bool InternalOnly { get; set; } = true;
        public DateTime? TimeStamp { get; set; }
        public string? EnteredByUserId { get; set; } = string.Empty;
    }
}
