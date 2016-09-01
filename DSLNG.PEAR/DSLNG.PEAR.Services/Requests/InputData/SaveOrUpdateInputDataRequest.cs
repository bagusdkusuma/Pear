using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.InputData
{
    public class SaveOrUpdateInputDataRequest
    {
        public SaveOrUpdateInputDataRequest()
        {
            GroupInputs = new List<GroupInput>();
            Kpis = new List<Kpi>();
        }

        public string Name { get; set; }
        public string PeriodeType { get; set; }
        
        public int Accountability { get; set; }
        public int Order { get; set; }
        public IList<GroupInput> GroupInputs { get; set; }
        public IList<Kpi> Kpis { get; set; }


        public class GroupInput
        {
            public GroupInput()
            {
                Kpis = new List<Kpi>();
            }

            public string Name { get; set; }
            public IList<Kpi> Kpis { get; set; }
            public int Order { get; set; }
        }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
        }
    }
}
