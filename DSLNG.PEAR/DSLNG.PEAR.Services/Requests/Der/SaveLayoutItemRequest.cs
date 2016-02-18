


using System.Collections.Generic;

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
        }

        public class LayoutItemArtifactLine
        {
            public LayoutItemArtifactLine()
            {
                Series = new List<LayoutItemArtifactSeries>();
            }

            public IList<LayoutItemArtifactSeries> Series { get; set; }
        }

        public class LayoutItemArtifactSeries
        {
            public int KpiId { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
        }
    }
}
