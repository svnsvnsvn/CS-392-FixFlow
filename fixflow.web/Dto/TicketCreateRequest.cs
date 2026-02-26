namespace fixflow.web.Dto;

public class TicketCreateRequest
{
    public int LocationCode { get; set; }
    public int Unit { get; set; }
    public int TicketTypeCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ResidentId { get; set; }
    public DateTime? DueDate { get; set; }
}
