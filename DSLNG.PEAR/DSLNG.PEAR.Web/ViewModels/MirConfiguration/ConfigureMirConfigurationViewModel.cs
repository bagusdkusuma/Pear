using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.MirConfiguration
{
    public class ConfigureMirConfigurationViewModel
    {
        public ConfigureMirConfigurationViewModel()
        {
            MirDataTables = new List<MirDataTable>();
            KpiList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public IList<MirDataTable> MirDataTables { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListItem> KpiList { get; set; }
        

        public class MirDataTable
        {
            public MirDataTable()
            {
                KpiIds = new List<int>();
            }
            public int Id { get; set; }
            public MirTableType Type { get; set; }
            public List<int> KpiIds { get; set; }
        }
    }
}