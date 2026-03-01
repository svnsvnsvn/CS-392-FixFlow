namespace fixflow.web.Dto
{
    public class StatusCodeDto
    {
        public int Id { get; set; }
        public int? StatusCode { get; set; } = 0;
        public string? StatusName { get; set; } = string.Empty;
    }
}
