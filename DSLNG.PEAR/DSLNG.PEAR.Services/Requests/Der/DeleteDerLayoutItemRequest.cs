namespace DSLNG.PEAR.Services.Requests.Der
{
    public class DeleteDerLayoutItemRequest : BaseRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
