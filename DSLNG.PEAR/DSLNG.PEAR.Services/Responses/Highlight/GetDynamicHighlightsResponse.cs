

using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Highlight
{
    public class GetDynamicHighlightsResponse
    {
        public GetDynamicHighlightsResponse() {
            HighlightGroups = new List<HighlightGroupResponse>();
            Periode = DateTime.Now;
        }
        public DateTime Periode { get; set; }
        public IList<HighlightGroupResponse> HighlightGroups { get; set; }
        public class HighlightGroupResponse
        {
            public HighlightGroupResponse()
            {
                Highlights = new List<HighlightResponse>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public IList<HighlightResponse> Highlights { get; set; }
        }
        public class HighlightResponse
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public int Order { get; set; }
            public string Type { get; set; }
            public int TypeId { get; set; }
        }
    }
}
