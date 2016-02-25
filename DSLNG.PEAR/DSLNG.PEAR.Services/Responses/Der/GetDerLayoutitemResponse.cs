using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDerLayoutitemResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int DerLayoutId { get; set; }
        public DerArtifact Artifact { get; set; }

        public class DerArtifact
        {
            public DerArtifact()
            {
                Series = new List<DerArtifactSerie>();
            } 
            public int Id { get; set; }
            public string HeaderTitle { get; set; }
            public int MeasurementId { get; set; }
            public string MeasurementName { get; set; }
            public string GraphicType { get; set; }

            public IList<DerArtifactSerie> Series { get; set; }
            public IList<DerArtifactChart> Charts { get; set; }
        }

        public class DerArtifactSerie
        {
            public int Id { get; set; }
            public string Label { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Color { get; set; }
        }

        public class DerArtifactChart
        {
            public int Id { get; set; }
            public string GraphicType { get; set; }
            public ICollection<DerArtifactSerie> Series { get; set; }
            public string ValueAxis { get; set; }
            public int MeasurementId { get; set; }
            public string ValueAxisTitle { get; set; }
            public string ValueAxisColor { get; set; }
            public double? FractionScale { get; set; }
            public double? MaxFractionScale { get; set; }
            public bool IsOpposite { get; set; }
        }
    }
}
