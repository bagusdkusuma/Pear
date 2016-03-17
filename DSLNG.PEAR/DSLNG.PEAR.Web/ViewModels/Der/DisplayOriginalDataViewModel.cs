using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class DisplayOriginalDataViewModel
    {
        public DisplayOriginalDataViewModel()
        {
            OriginalData = new List<OriginalDataViewModel>();
        }

        public int Id { get; set; }
        public DateTime CurrentDate { get; set; } 
        public IList<OriginalDataViewModel> OriginalData { get; set; }

        public class OriginalDataViewModel
        {
            public int Id { get; set; }
            public int LayoutItemId { get; set; }
            public string Data { get; set; }
            public string DataType { get; set; }
            public DateTime Periode { get; set; }
            public string PeriodeType { get; set; }
            public string Type { get; set; }
            public int Position { get; set; }
            public bool IsKpiAchievement { get; set; }
            public string Label { get; set; }
            public int KpiId { get; set; }
        }
    }
}