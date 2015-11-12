

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.HighlightGroup
{
    public class GetHighlightGroupsResponse
    {
        public IList<HighlightGroupResponse> HighlightGroups { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class HighlightGroupResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
        }
    }
}
