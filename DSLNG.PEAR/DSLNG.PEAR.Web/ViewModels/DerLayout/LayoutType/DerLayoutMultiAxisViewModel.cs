using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Artifact;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType
{
    public class DerLayoutMultiAxisViewModel
    {
        [Display(Name = "Header Title")]
        public string HeaderTitle { get; set; }
        
        public LineChartViewModel LineChart { get; set; }

        public MultiaxisChartViewModel MultiaxisChart { get; set; }

    }
}