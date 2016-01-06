

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.HighlightGroup
{
    public class GetHighlightGroupsResponse
    {
        public IList<HighlightGroupResponse> HighlightGroups { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class HighlightGroupResponse {
            public HighlightGroupResponse() {
                HighlightTypes = new List<HighlightTypeResponse>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public IList<HighlightTypeResponse> HighlightTypes { get; set; }
        }
        public class HighlightTypeResponse {
            public int Id { get; set; }
            public string Text { get; set; }
            public int Order { get; set; }
            public string Value { get; set; }
            public bool IsActive { get; set; }
            public int[] RoleGroupIds { get; set; }
        }
    }
}
