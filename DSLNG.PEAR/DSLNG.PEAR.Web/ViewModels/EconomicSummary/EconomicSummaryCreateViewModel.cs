using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.EconomicSummary
{
    public class EconomicSummaryCreateViewModel
    {
        public EconomicSummaryCreateViewModel()
        {
            Scenarios = new List<Scenario>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public IList<Scenario> Scenarios { get; set; } 

        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}