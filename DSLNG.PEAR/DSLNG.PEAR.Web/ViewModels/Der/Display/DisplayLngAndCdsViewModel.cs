﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayLngAndCdsViewModel
    {
        public DisplayLngAndCdsViewModel()
        {
            DisplayLngAndCds = new List<LngAndCdsViewModel>();    
        }

        public IList<LngAndCdsViewModel> DisplayLngAndCds { get; set; }

        public class LngAndCdsViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string ActualMtd { get; set; }
            public string ActualYtd { get; set; }
            public string TargetMtd { get; set; }
            public string TargetYtd { get; set; }
        }
    }
}