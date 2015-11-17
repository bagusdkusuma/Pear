
namespace DSLNG.PEAR.Services.Requests.CalculatorConstant
{
    public class GetCalculatorConstantsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string Term { get; set; }
        public bool OnlyCount { get; set; }
    }
}
