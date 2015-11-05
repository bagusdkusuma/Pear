



using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.Select
{
    public class GetSelectResponse : BaseResponse
    {
        public GetSelectResponse()
        {
            Options = new List<SelectOptionResponse>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IList<SelectOptionResponse> Options { get; set; }
        public bool IsActive { get; set; }
        public SelectType Type { get; set; }
        public int ParentId { get; set; }

        public class SelectOptionResponse
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public string Text { get; set; }
        }
    }
}
