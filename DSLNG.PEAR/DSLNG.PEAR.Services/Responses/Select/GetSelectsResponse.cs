



using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.Select
{
    public class GetSelectsResponse
    {
        public IList<Select> Selects { get; set; }
        public int TotalRecords { get; set; }
        public class Select 
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Options { get; set; }
            public bool IsActive { get; set; }
            public string Parent { get; set; }
            public string ParentOption { get; set; }
        }
    }
}
