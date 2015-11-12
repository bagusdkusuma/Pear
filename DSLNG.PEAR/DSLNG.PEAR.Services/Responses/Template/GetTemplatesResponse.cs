

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Template
{
    public class GetTemplatesResponse
    {
        public IList<TemplateResponse> Artifacts { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class TemplateResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
