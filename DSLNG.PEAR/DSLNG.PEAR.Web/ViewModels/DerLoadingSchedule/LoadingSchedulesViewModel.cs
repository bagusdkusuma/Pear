using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.DerLoadingSchedule
{
    public class LoadingSchedulesViewModel
    {
        public IList<LoadingScheduleViewModel> Schedules { get; set; }
        public class LoadingScheduleViewModel
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string VesselType { get; set; }
            public string Vessel { get; set; }
            public double Capacity { get; set; }
            public DateTime ETA { get; set; }
            public DateTime ETD { get; set; }
            public string Cargo { get; set; }
            public string Remark { get; set; }
            public string Buyer { get; set; }
            public DateTime? RemarkDate { get; set; }
            public string Measurement { get; set; }
            public string SalesType { get; set; }
        }
    }
}