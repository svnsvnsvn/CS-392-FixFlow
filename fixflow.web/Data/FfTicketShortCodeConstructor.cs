namespace fixflow.web.Data
{
    public class FfTicketShortCodeConstructor
    {
        public int Id { get; set; }
        public string TicketPrefix { get; set; } = string.Empty;
        public bool SeriesIsActive { get; set; }
        public short TicketSeries { get; set; }
        public int LastTicketUsed { get; set; }
    }
}
