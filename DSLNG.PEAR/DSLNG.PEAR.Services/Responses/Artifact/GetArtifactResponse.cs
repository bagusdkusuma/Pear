

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class GetArtifactResponse
    {
        public GetArtifactResponse()
        {
            Series = new List<SeriesResponse>();
            PlotBands = new List<PlotResponse>();
            Charts = new List<ChartResponse>();
        }
        public int Id { get; set; }
        public string GraphicType { get; set; }
        public string GraphicName { get; set; }
        public string HeaderTitle { get; set; }

        public IList<SeriesResponse> Series { get; set; }
        public IList<PlotResponse> PlotBands { get; set; }
        public IList<RowResponse> Rows { get; set; }
        public IList<ChartResponse> Charts { get; set; }
        public TankResponse Tank { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public string Measurement { get; set; }
        public int MeasurementId { get; set; }
        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public bool AsNetbackChart { get; set; }

        public bool Actual { get; set; }
        public bool Target { get; set; }
        public bool Economic { get; set; }
        public bool Fullfillment { get; set; }
        public bool Remark { get; set; }
        public bool Is3D { get; set; }
        public bool ShowLegend { get; set; }
        public string SeriesType { get; set; }

        public class SeriesResponse
        {
            public SeriesResponse()
            {
                Stacks = new List<StackResponse>();
            }
            public string Color { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Label { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public IList<StackResponse> Stacks { get; set; }
            public string MarkerColor { get; set; }
            public string LineType { get; set; }
        }
        public class StackResponse
        {
            public string Color { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Label { get; set; }
        }
        public class PlotResponse
        {
            public double From { get; set; }
            public double To { get; set; }
            public string Color { get; set; }
            public string Label { get; set; }
        }

        public class RowResponse
        {
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public RangeFilter RangeFilter { get; set; }
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
        }

        public class TankResponse
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
            public bool ShowLine{ get; set; }
        }


        public class ChartResponse
        {
            public int MeasurementId { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public string GraphicType { get; set; }
            public IList<SeriesResponse> Series { get; set; }
            public string ValueAxisTitle { get; set; }
            public string ValueAxisColor { get; set; }
            public bool IsOpposite { get; set; }
            public double? FractionScale { get; set; }
            public double? MaxFractionScale { get; set; }
            public string SeriesType { get; set; }
        }

    }
}
