using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.MirDataTable
{
    public class SaveMirDataTableViewModel
    {
        public int MirDataId { get; set; }
        public int MirConfigurationId { get; set; }
        public int[] KpiIds { get; set; }
        public MirTableType Type { get; set; }
    }
}