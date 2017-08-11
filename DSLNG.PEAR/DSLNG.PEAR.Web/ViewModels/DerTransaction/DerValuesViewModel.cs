
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Web.ViewModels.Wave;
using DSLNG.PEAR.Web.ViewModels.Weather;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.DerTransaction
{
    public class DerValuesViewModel
    {
        public DerValuesViewModel() {
            Highlights = new List<DerHighlightValuesViewModel>();
            KpiInformations = new List<KpiInformationValuesViewModel>();
        }
        public IList<DerHighlightValuesViewModel> Highlights { get; set; }
        public IList<KpiInformationValuesViewModel> KpiInformations { get; set; }
        public class DerHighlightValuesViewModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public string Text { get; set; }
            public int HighlightTypeId { get; set; }
            public string HighlightTypeValue { get; set; }
            public DateTime Date { get; set; }
            public string HighlightMessage { get; set; }
            public string HighlightTitle { get; set; }
            public string Type { get; set; }
        }

        public class KpiInformationValuesViewModel
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public int Position { get; set; }
            public ConfigType ConfigType { get; set; }
            public KpiValueViewModel DailyTarget { get; set; }
            public KpiValueViewModel MonthlyTarget { get; set; }
            public KpiValueViewModel YearlyTarget { get; set; }

            public KpiValueViewModel DailyActual { get; set; }
            public KpiValueViewModel MonthlyActual { get; set; }
            public KpiValueViewModel YearlyActual { get; set; }
        }
        public class KpiValueViewModel
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public string Type { get; set; }
        }
        public WaveViewModel Wave { get; set; }
        public WeatherViewModel Weather { get; set; }
        public IList<SelectListItem> AlertOptions { get; set; }
        public DerLoadingScheduleViewModel DerLoadingSchedule {get;set;}
        public class DerLoadingScheduleViewModel {
            public string ExistValueTime { get; set; }
            public IList<VesselScheduleViewModel> VesselSchedules { get; set; }
            public class VesselScheduleViewModel
            {
                public VesselScheduleViewModel()
                {
                }
                public int id { get; set; }
                public string Vessel { get; set; }
                public string Name { get; set; }
                public DateTime? ETA { get; set; }
                public DateTime? ETD { get; set; }
                public bool IsActive { get; set; }
                public string Buyer { get; set; }
                public string Location { get; set; }
                public string SalesType { get; set; }
                public string Type { get; set; }
                public string VesselType { get; set; }
                public string Cargo { get; set; }
                public string Remark { get; set; }
                public DateTime? RemarkDate { get; set; }
                public string Measurement { get; set; }
                public double Capacity { get; set; }
            }
        }
    }
}