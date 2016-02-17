
namespace DSLNG.PEAR.Web.ViewModels.BusinessPosture
{
    public class PostureChallengeViewModel
    {
        public int Id { get; set; }
        public int PostureId { get; set; }
        public string Definition { get; set; }
        public int[] RelationIds { get; set; }
    }
}