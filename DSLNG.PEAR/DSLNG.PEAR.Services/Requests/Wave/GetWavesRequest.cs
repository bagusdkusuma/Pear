
namespace DSLNG.PEAR.Services.Requests.Wave
{
    public class GetWavesRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
