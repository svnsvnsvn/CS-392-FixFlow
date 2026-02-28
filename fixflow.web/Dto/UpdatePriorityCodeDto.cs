namespace fixflow.web.Dto
{
    public class UpdatePriorityCodeDto
    {
        public int Id { get; set; }
        public int? PriorityCode { get; set; } = 0;
        public string? PriorityName { get; set; } = string.Empty;
    }
}
