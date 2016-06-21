using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class DerLayoutItemViewModel
    {
        private readonly IDropdownService _dropdownService;
        public DerLayoutItemViewModel()
        {
            _dropdownService = ObjectFactory.Container.GetInstance<IDropdownService>();
            KpiInformations = new List<DerKpiInformationViewModel>();
        }

        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Type { get; set; }
        public string OldType { get; set; }
        public int DerLayoutId { get; set; }
        public IList<SelectListItem> Types { get; set; }

        public DerLayoutItemArtifactViewModel Artifact { get; set; }
        public LineChartViewModel LineChart { get; set; }
        public MultiaxisChartViewModel MultiaxisChart { get; set; }
        public PieViewModel Pie { get; set; }
        public TankViewModel Tank { get; set; }
        public SpeedometerChartViewModel SpeedometerChart { get; set; }

        [Display(Name = "Highlight")]
        public int HighlightId { get; set; }
        public IList<SelectListItem> Highlights { get; set; }

        public IList<DerKpiInformationViewModel> KpiInformations { get; set; }
        public IList<SelectListItem> ConfigTypes
        {
            get
            {
                return _dropdownService.GetConfigTypes(true)
                                    .Select(x => new SelectListItem { Text = x.Text, Value = x.Value })
                                    .ToList();
            }
        }

        [Display(Name = "User")]
        public int SignedBy { get; set; }
        public IList<SelectListItem> Users { get; set; }

        public class DerKpiInformationViewModel
        {
            public int Id { get; set; }
            public int Position { get; set; }
            public int KpiId { get; set; }
            public string KpiLabel { get; set; }
            public string KpiName { get; set; }
            public string KpiMeasurement { get; set; }
            public string ConfigType { get; set; }
            public int HighlightId { get; set; }
        }

        public class DerLayoutItemArtifactViewModel
        {
            public int Id { get; set; }
            [Display(Name = "Header Title")]
            public string HeaderTitle { get; set; }
            [Display(Name = "Measurement")]
            public int MeasurementId { get; set; }
            public IList<SelectListItem> Measurements { get; set; }

            public bool Is3D { get; set; }
            public bool ShowLegend { get; set; }
        }

    }
}
