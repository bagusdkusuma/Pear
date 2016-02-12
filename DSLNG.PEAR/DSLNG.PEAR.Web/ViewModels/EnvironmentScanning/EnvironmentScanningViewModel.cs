using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.EnvironmentScanning
{
    public class EnvironmentScanningViewModel
    {
        public EnvironmentScanningViewModel()
        {
            ConstructionPhase = new List<UltimateObjective>();
            OperationPhase = new List<UltimateObjective>();
            ReinventPhase = new List<UltimateObjective>();
            Threat = new List<Environmental>();
            Opportunity = new List<Environmental>();
            Weakness = new List<Environmental>();
            Strength = new List<Environmental>();
        }


        public int Id { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
        public IList<UltimateObjective> ConstructionPhase { get; set; }
        public IList<UltimateObjective> OperationPhase { get; set; }
        public IList<UltimateObjective> ReinventPhase { get; set; }
        public IList<Environmental> Threat { get; set; }
        public IList<Environmental> Opportunity { get; set; }
        public IList<Environmental> Weakness { get; set; }
        public IList<Environmental> Strength { get; set; }


        public class UltimateObjective
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class Environmental
        {
            public int Id { get; set; }
            public string Desc { get; set;  }
        }

        public class CreateViewModel
        {
            public int Id { get; set; }
            public int EsId { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
        }

        public class DeleteViewModel
        {
            public int Id { get; set; }
        }

        public class CreateEnvironmentalViewModel
        {
            public int Id { get; set; }
            public int EnviId { get; set; }
            public string Description { get; set; }
            public string EnviType { get; set; }
        }
    }
}