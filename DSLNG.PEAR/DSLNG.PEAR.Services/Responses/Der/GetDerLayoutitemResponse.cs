using System.Collections.Generic;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDerLayoutitemResponse : BaseResponse
    {
        public GetDerLayoutitemResponse()
        {
            KpiInformations = new List<KpiInformationResponse>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int DerLayoutId { get; set; }
        public DerArtifact Artifact { get; set; }
        public DerHighlight Highlight { get; set; }
        public IList<KpiInformationResponse> KpiInformations { get; set; }
        public int SignedBy { get; set; }

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
            public bool Is3D { get; set; }
            public bool ShowLegend { get; set; }

            public IList<DerArtifactSerie> Series { get; set; }
            public IList<DerArtifactChart> Charts { get; set; }
            public DerArtifactTank Tank { get; set; }

            public KpiResponse CustomSerie { get; set; }
            public IList<DerArtifactPlot> Plots { get; set; }


        }

        public class KpiResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string MeasurementName { get; set; }
        }

        public class DerArtifactPlot
        {
            public int Id { get; set; }
            public double From { get; set; }
            public double To { get; set; }
            public string Color { get; set; }
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

        public class DerArtifactTank
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

        public class DerHighlight
        {
            public int Id { get; set; }
            public int SelectOptionId { get; set; }
        }

        public class KpiInformationResponse
        {
            public int Id { get; set; }
            public KpiResponse Kpi { get; set; }
            public int Position { get; set; }
            public string KpiLabel { get; set; }
            public string KpiMeasurement { get; set; }
            public ConfigType ConfigType { get; set; }
            public SelectOptionResponse SelectOption { get; set; }

            public class KpiResponse
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string MeasurementName { get; set; }
            }

            public class SelectOptionResponse
            {
                public int Id { get; set; }
                public string Value { get; set; }
                public string Text { get; set; }
            }
        }

        //public class SignedBy
        //{
        //    public string FullName { get; set; }
        //    public string SignatureImage { get; set; }
        //}

    }
}
