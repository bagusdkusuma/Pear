using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EconomicSummary
{
    [Serializable]
    public class GetEconomicSummaryReportResponse
    {
        public GetEconomicSummaryReportResponse()
        {
            Scenarios = new List<Scenario>();
            Groups = new List<Group>();
        }

        public List<Scenario> Scenarios { get; set; }
        public List<Group> Groups { get; set; }
        
        [Serializable]
        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        [Serializable]
        public class OutputResult
        {
            public string Actual { get; set; }
            public string Forecast { get; set; }
            //public Scenario Scenario { get; set; }
        }
        [Serializable]
        public class KeyOutput
        {
            public KeyOutput()
            {
                //OutputResults = new List<OutputResult>();
            }
            public string Name { get; set; }
            public string Measurement { get; set; }
            //public List<OutputResult> OutputResults { get; set; }
            //public IDictionary<int, List<OutputResult>> Results { get; set; } 
            public OutputResult OutputResult { get; set; }
            public Scenario Scenario { get; set; }
        }
        [Serializable]
        public class Group
        {
            public Group()
            {
                KeyOutputs = new List<KeyOutput>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public List<KeyOutput> KeyOutputs { get; set; }
        }
    }
}
