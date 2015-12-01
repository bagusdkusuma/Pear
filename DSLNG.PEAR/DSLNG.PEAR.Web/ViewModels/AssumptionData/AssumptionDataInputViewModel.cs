
using System.Collections.Generic;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.AssumptionData
{
    public class AssumptionDataInputViewModel
    {
        public ScenarioViewModel Scenario { get; set; }
        public IList<AssumptionCategoryViewModel> KeyAssumptionCategories { get; set; }
        public IList<AssumptionDataViewModel> AssumptionDataList { get; set; }
        public class ScenarioViewModel {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class AssumptionCategoryViewModel
        {
            public AssumptionCategoryViewModel()
            {
                Assumptions = new List<AssumptionViewModel>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public int Order { get; set; }
            public IList<AssumptionViewModel> Assumptions { get; set; }
        }
        public class AssumptionViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Measurement { get; set; }
            public bool IsActive {get;set;}
        }
        public class AssumptionDataViewModel
        {
            public int Id { get; set; }
            public int IdScenario { get; set; }
            public int IdConfig { get; set; }
            public string ActualValue { get; set; }
            public string ForecastValue { get; set; }
        }
    }
}