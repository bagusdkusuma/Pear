using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Artifact;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType
{
    public class DerLayoutLineViewModel
    {
        [Display(Name = "Header Title")]
        public string HeaderTitle { get; set; }
        [Display(Name = "Measurement")]
        public int MeasurementId { get; set; }
        public IList<SelectListItem> Measurements { get; set; }

        public LineChartViewModel LineChart { get; set; }

    }
}