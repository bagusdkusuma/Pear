
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class PlanningBlueprintViewModel
    {
        public PlanningBlueprintViewModel() {
            IsActive = true;
            KeyOutputs = new List<KeyOutputViewModel>();
        }
        public bool IsReviewer { get; set; }
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
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