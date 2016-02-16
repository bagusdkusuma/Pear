

using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class EnvironmentsScanningViewModel
    {
        public UltimateObjectViewModel UltimateObject { get; set; }
        public EnvironmentalScanningViewModel EnvironmentalScanning { get; set; }
        public IList<ConstraintViewModel> Constraints { get; set; }
        public IList<ChallangeViewModel> Challanges { get; set; }
        public class UltimateObjectViewModel {
            public int Id { get; set; }
            public IList<DescriptionViewModel> ConstructionPhase { get; set; }
            public IList<DescriptionViewModel> Operationphase { get; set; }
            public IList<DescriptionViewModel> ReinventPhase { get; set; }
        }
        public class EnvironmentalScanningViewModel {
            public int Id { get; set; }
            public IList<DescriptionViewModel> Threat { get; set; }
            public IList<DescriptionViewModel> Opportunity { get; set; }
            public IList<DescriptionViewModel> Weakness { get; set; }
            public IList<DescriptionViewModel> Strength { get; set; }
        }

        public class DescriptionViewModel {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class ConstraintViewModel {
            public int Id { get; set; }
            public string Definition { get; set; }
            public string Type { get; set; }
            public string Category { get; set; }
            public IList<DescriptionViewModel> Threat { get; set; }
            public IList<DescriptionViewModel> Opportunity { get; set; }
            public IList<DescriptionViewModel> Weakness { get; set; }
            public IList<DescriptionViewModel> Strength { get; set; }
        }
        public class ChallangeViewModel {
            public int Id { get; set; }
            public string Definition { get; set; }
            public string Type { get; set; }
            public string Category { get; set; }
            public IList<DescriptionViewModel> Threat { get; set; }
            public IList<DescriptionViewModel> Opportunity { get; set; }
            public IList<DescriptionViewModel> Weakness { get; set; }
            public IList<DescriptionViewModel> Strength { get; set; }
        }

    }
}