using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OperationData
{
    public class OperationDataParamConfigurationViewModel
    {
        public int RoleGroupId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string PeriodeType { get; set; }
        public int ScenarioId { get; set; }
    }
}