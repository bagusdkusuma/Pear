
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class PlanningBlueprintViewModel
    {
        public PlanningBlueprintViewModel() {
            IsActive = true;
            KeyOutputs = new List<KeyOutputViewModel>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int[] KeyOutputIds { get; set; }
        public IList<KeyOutputViewModel> KeyOutputs { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }

        public class KeyOutputViewModel {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}