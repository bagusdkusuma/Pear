


using System.Collections.Generic;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class SaveLayoutItemRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int DerLayoutId { get; set; }

        public LayoutItemArtifact Artifact { get; set; }

        public class LayoutItemArtifact
        {
            public string HeaderTitle { get; set; }
            public int MeasurementId { get; set; }
            public LayoutItemArtifactLine LineChart { get; set; }
            public LayoutItemArtifactMultiAxis MultiAxis { get; set; }
        }

        public class LayoutItemArtifactLine
        {
            public LayoutItemArtifactLine()
            {
                Series = new List<LayoutItemArtifactSerie>();
            }

            public IList<LayoutItemArtifactSerie> Series { get; set; }
        }

        public class LayoutItemArtifactMultiAxis
        {
            public LayoutItemArtifactMultiAxis()
            {
                Charts = new List<LayoutItemArtifactChart>();
            }

            public IList<LayoutItemArtifactChart> Charts { get; set; }
        }

        public class LayoutItemArtifactChart
        {
            public int MeasurementId { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public string GraphicType { get; set; }
            public IList<LayoutItemArtifactSerie> Series { get; set; }
            public string ValueAxisTitle { get; set; }
            public string ValueAxisColor { get; set; }
            public bool IsOpposite { get; set; }
            public double? FractionScale { get; set; }
            public double? MaxFractionScale { get; set; }
        }

        public class LayoutItemArtifactSerie
        {
            public int KpiId { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
        }

    }
}
