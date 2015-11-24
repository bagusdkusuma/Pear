using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Scenario
{
    public class ScenarioViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsDashboard { get; set; }
    }
}