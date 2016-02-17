
namespace DSLNG.PEAR.Services.Responses.BusinessPosture
{
    public class SavePostureConstraintResponse : BaseResponse
    {
        public int Id { get; set; }
        public int[] RelationIds { get; set; }
        public string Definition { get; set; }
        public int PostureId { get; set; }
    }
}
