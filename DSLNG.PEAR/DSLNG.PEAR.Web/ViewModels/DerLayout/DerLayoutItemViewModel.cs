using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class DerLayoutItemViewModel
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Type { get; set; }
        public int DerLayoutId { get; set; }
        public IList<SelectListItem> Types { get; set; }

        public DerLayoutItemArtifactViewModel Artifact { get; set; }
        public LineChartViewModel LineChart { get; set; }
        public MultiaxisChartViewModel MultiaxisChart { get; set; }

        //public DerLayoutLineViewModel Artifact { get; set; }

        /* public DerLayoutArtifactViewModel Line { get; set; }
         public int MeasurementId { get; set; }

         public LineChartViewModel LineChart { get; set; }*/

        //public DerLayoutLineViewModel Line { get; set; }
        //public LineChartViewModel LineChart { get; set; }

        public class DerLayoutItemArtifactViewModel
        {
            [Display(Name = "Header Title")]
            public string HeaderTitle { get; set; }
            [Display(Name = "Measurement")]
            public int MeasurementId { get; set; }
            public IList<SelectListItem> Measurements { get; set; }
        }
    }
}
