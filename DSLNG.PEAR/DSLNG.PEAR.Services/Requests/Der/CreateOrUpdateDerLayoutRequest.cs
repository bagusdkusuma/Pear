


namespace DSLNG.PEAR.Services.Requests.Der
{
    public class CreateOrUpdateDerLayoutRequest : BaseRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
    }
}
