


using System.Collections.Generic;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class SaveLayoutItemRequest
    {
        public SaveLayoutItemRequest()
        {
            KpiInformations = new List<DerKpiInformationRequest>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string OldType { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int DerLayoutId { get; set; }
        public LayoutItemHighlight Highlight { get; set; }
        public LayoutItemArtifact Artifact { get; set; }
        public int SignedBy { get; set; }

        public class LayoutItemHighlight
        {
            public int Id { get; set; }
            public int SelectOptionId { get; set; }
        }

        public class LayoutItemArtifact
        {
            public int Id { get; set; }
            public string HeaderTitle { get; set; }
            public int MeasurementId { get; set; }
            public bool Is3D { get; set; }
            public bool ShowLegend { get; set; }
            
            public LayoutItemArtifactLine LineChart { get; set; }
            public LayoutItemArtifactMultiAxis MultiAxis { get; set; }
            public LayoutItemArtifactPie Pie { get; set; }
            public LayoutItemArtifactTank Tank { get; set; }
            public LayoutItemArtifactSpeedometer Speedometer { get; set; }
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

        public class LayoutItemArtifactPie
        {
            public LayoutItemArtifactPie()
            {
                Series = new List<LayoutItemArtifactSerie>();
            }
            
            public IList<LayoutItemArtifactSerie> Series { get; set; }
        }

        public class LayoutItemArtifactSpeedometer
        {
            public LayoutItemArtifactSpeedometer()
            {
                Series = new LayoutItemArtifactSerie();
                PlotBands = new List<LayoutItemPlotBand>();
            }
            public LayoutItemArtifactSerie Series { get; set; }
            public LayoutItemArtifactSerie LabelSeries { get; set; }
            public IList<LayoutItemPlotBand> PlotBands { get; set; }
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

        public class LayoutItemPlotBand
        {
            public double From { get; set; }
            public double To { get; set; }
            public string Color { get; set; }
        }

        public class LayoutItemArtifactTank
        {
            public int Id { get; set; }
            public int VolumeInventoryId { get; set; }
            public string VolumeInventory { get; set; }
            public int DaysToTankTopId { get; set; }
            public string DaysToTankTop { get; set; }
            public string DaysToTankTopTitle { get; set; }
            public double MinCapacity { get; set; }
            public double MaxCapacity { get; set; }
            public string Color { get; set; }
            public bool ShowLine { get; set; }
        }

        public IList<DerKpiInformationRequest> KpiInformations { get; set; }

        public class DerKpiInformationRequest
        {
            public int Id { get; set; }
            public int Position { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string KpiLabel { get; set; }
            public string KpiMeasurement{ get; set; }
            public ConfigType ConfigType { get; set; }
            public int HighlightId { get; set; }
        }
    }
}
