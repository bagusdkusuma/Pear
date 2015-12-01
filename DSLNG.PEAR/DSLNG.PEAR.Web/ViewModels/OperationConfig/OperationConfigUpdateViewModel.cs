using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OperationConfig
{
    public class OperationConfigUpdateViewModel
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; }
        public int KeyOperationGroupId { get; set; }
        public int KpiId { get; set; }
    }
}