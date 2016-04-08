using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.File
{
    public class DownloadTemplateViewModel
    {
        public string ConfigType { get; set; }
        public string PeriodeType { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int RoleGroupId { get; set; }
        public int ScenarioId { get; set; }
        //public int GroupId { get; set; }
    }
}