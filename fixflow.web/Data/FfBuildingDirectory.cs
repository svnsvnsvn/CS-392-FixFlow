using System.Drawing;

namespace fixflow.web.Data
{
    public class FfBuildingDirectory
    {
        public int LocationCode { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string ComplexName {  get; set; } = string.Empty;
        public int BuildingNumber { get; set; }
        public int NumUnits { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLon { get; set; }
    }
}
