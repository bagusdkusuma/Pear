

namespace DSLNG.PEAR.Services.Requests.Buyer
{
    public class SaveBuyerRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
