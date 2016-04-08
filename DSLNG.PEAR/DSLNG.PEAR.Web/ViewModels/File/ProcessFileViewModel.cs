using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.File
{
    public class ProcessFileViewModel
    {
        public string ConfigType { get; set; }
        public string Filename { get; set; }
        public int ScenarioId { get; set; }
        public int OperationId { get; set; }
    }
}