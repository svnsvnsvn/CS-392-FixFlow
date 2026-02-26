namespace fixflow.web.Dto
{
    public class UpdateBuildingDto
    {
<<<<<<< HEAD
        public int LocationCode { get; set; }
        public string? LocationName { get; set; } = string.Empty;
        public string? ComplexName { get; set; } = string.Empty;
        public int? BuildingNumber { get; set; }
        public int? NumUnits { get; set; }
=======
        public int LocationCode { get; set; } = 0;
        public string LocationName { get; set; } = string.Empty;
        public string ComplexName { get; set; } = string.Empty;
        public int BuildingNumber { get; set; }
        public int NumUnits { get; set; }
>>>>>>> origin/ZZ-development/dashboard-refresh
        public decimal? LocationLat { get; set; }
        public decimal? LocationLon { get; set; }
    }
}
