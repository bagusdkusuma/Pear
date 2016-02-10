
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class PlanningBlueprintViewModel
    {
        public PlanningBlueprintViewModel() {
            IsActive = true;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
    }
}