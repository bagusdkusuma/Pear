



using System.Collections.Generic;

namespace DSLNG.PEAR.Web.ViewModels.EconomicSummary
{
    public class EconomicSummaryReportViewModel
    {
        public EconomicSummaryReportViewModel()
        {
            Scenarios = new List<Scenario>();
            Groups = new List<Group>();
        }
        public IList<Scenario> Scenarios { get; set; }
        public IList<Group> Groups { get; set; }

        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class OutputResult
        {
            public string Actual { get; set; }
            public string Forecast { get; set; }
            //public Scenario Scenario { get; set; }
        }

        public class KeyOutput
        {
            public KeyOutput()
            {
                //OutputResults = new List<OutputResult>();
            }
            public string Name { get; set; }
            public string Measurement { get; set; }
            //public IList<OutputResult> OutputResults { get; set; }
            //public IDictionary<int, IList<OutputResult>> Results { get; set; } 
            public OutputResult OutputResult { get; set; }
            public Scenario Scenario { get; set; }
        }

        public class Group
        {
            public Group()
            {
                KeyOutputs = new List<KeyOutput>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public IList<KeyOutput> KeyOutputs { get; set; }
        }
    }
}
