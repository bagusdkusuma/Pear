using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OperationData
{
    public class UpdateOperationDataViewModel
    {
        public int Id { get; set; }
        public int ScenarioId    { get; set; }
        public int KeyOperationConfigId { get; set; }
        public int KpiId { get; set; }
        public double? Value { get; set; }
        public string Remark { get; set; }
        public DateTime Periode { get; set; }
        public string PeriodeType { get; set; }
    }
}