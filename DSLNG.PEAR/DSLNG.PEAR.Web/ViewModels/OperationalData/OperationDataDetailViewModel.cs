using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OperationalData
{
    public class OperationDataDetailViewModel
    {
        public OperationDataDetailViewModel()
        {
            KeyOperationGroups = new List<KeyOperationGroupViewModel>();    
        }

        public IList<KeyOperationGroupViewModel> KeyOperationGroups { get; set; }
        public int ScenarioId { get; set; }
        public string ConfigType { get; set; }

        public class KeyOperationGroupViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
            public IList<KeyOperationConfigViewModel> KeyOperationConfigs { get; set; }
        }

        public class KeyOperationConfigViewModel
        {
            public int Id { get; set; }
            public KpiViewModel Kpi { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public int Order { get; set; }
        }

        public class KpiViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string MeasurementName { get; set; }
        }
    }
}