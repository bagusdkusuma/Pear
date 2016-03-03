using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.PopDashboard
{
    public class PopInformationViewModel
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public PopInformationType Type { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class SavePopInformationViewModel
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }
}