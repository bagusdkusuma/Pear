

namespace DSLNG.PEAR.Services.Requests.HighlightOrder
{
    public class SaveHighlightOrderRequest
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public int GroupId { get; set; }
        public int[] RoleGroupIds { get; set; }
        public bool? IsActive { get; set; }
    }
}
