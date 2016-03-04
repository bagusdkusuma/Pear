using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDerLayoutsResponse : BaseResponse
    {
        public IList<GetDerLayoutsResponse.DerLayout> DerLayouts { get; set; }
        
        public class DerLayout
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
