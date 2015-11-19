
namespace DSLNG.PEAR.Services.Requests.Weather
{
    public class GetWeathersRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
