

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class CreateArtifactRequest : BaseRequest
    {
        public CreateArtifactRequest()
        {
            Series = new List<SeriesRequest>();
            Plots = new List<PlotRequest>();
            Rows = new List<RowRequest>();
            Charts = new List<ChartRequest>();
        }

        public int Id { get; set; }
        public string GraphicName { get; set; }
        public string GraphicType { get; set; }
        public string HeaderTitle { get; set; }
        public IList<SeriesRequest> Series { get; set; }
        public IList<PlotRequest> Plots { get; set; }
        public IList<RowRequest> Rows { get; set; }
        public IList<ChartRequest> Charts { get; set; }
        public TankRequest Tank { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public int MeasurementId { get; set; }
        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public double MaxValue { get; set; }
        //tabular
        public bool Actual { get; set; }
        public bool Target { get; set; }
        public bool Economic { get; set; }
        public bool Fullfillment { get; set; }
        public bool Remark { get; set; }
        public bool Is3D { get; set; }
        public bool ShowLegend { get; set; }
        public bool AsNetbackChart { get; set; }
        public string SeriesType { get; set; }

        public class SeriesRequest
        {
            public SeriesRequest()
            {
                Stacks = new List<StackRequest>();
            }
            public string Label { get; set; }
            public IList<StackRequest> Stacks { get; set; }
            public string Color { get; set; }
            public int KpiId { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public string PreviousColor { get; set; }
            public string MarkerColor { get; set; }
            public string LineType { get; set; }
        }

        public class PlotRequest
        {
            public double From { get; set; }
            public double To { get; set; }
            public string Color { get; set; }
            public string Label { get; set; }
        }

        public class StackRequest
        {
            public string Label { get; set; }
            public int KpiId { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public string Color { get; set; }

        }

        public class RowRequest
        {
            public int KpiId { get; set; }
            public string PeriodeType { get; set; }
            public string RangeFilter { get; set; }
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
        }

        public class TankRequest
        {
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

        public class ChartRequest
        {
            public int MeasurementId { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public string GraphicType { get; set; }
            public IList<SeriesRequest> Series { get; set; }
            public string ValueAxisTitle { get; set; }
            public string ValueAxisColor { get; set; }
            public bool IsOpposite { get; set; }
            public double? FractionScale { get; set; }
            public double? MaxFractionScale { get; set; }
            public string SeriesType { get; set; }
            //public class SeriesRequest
            //{
            //    public SeriesRequest()
            //    {
            //        Stacks = new List<StackRequest>();
            //    }
            //    public int KpiId { get; set; }
            //    public string Label { get; set; }
            //    public ValueAxis ValueAxis { get; set; }
            //    public IList<StackRequest> Stacks { get; set; }
            //    public string Color { get; set; }
            //    public string PreviousColor { get; set; }
            //}

            //public class StackRequest
            //{
            //    public int KpiId { get; set; }
            //    public string Label { get; set; }
            //    public string Color { get; set; }
            //}
        }
    }
}
