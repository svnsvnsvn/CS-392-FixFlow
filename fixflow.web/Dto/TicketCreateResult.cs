namespace fixflow.web.Dto;

public class TicketCreateResult
{
    public Guid TicketId { get; set; }
    public string TicketShortCode { get; set; } = string.Empty;
}
