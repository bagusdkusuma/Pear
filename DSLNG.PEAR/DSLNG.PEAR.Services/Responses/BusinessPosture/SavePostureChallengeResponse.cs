
namespace DSLNG.PEAR.Services.Responses.BusinessPosture
{
    public class SavePostureChallengeResponse : BaseResponse
    {
        public int Id { get; set; }
        public int[] RelationIds { get; set; }
        public string Definition { get; set; }
    }
}
