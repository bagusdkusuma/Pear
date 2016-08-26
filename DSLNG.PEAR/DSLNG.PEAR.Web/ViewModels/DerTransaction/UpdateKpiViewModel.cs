using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.DerTransaction
{
    public class UpdateKpiOriginalViewModel
    {
        public int Id { get; set; }
        public int KpiId { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}