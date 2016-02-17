
namespace DSLNG.PEAR.Services.Requests.BusinessPosture
{
    public class SavePostureConstraintRequest
    {
        public int Id { get; set; }
        public int PostureId { get; set; }
        public int[] RelationIds { get; set; }
        public string Definition { get; set; }
    }
}
