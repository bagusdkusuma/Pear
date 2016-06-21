using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Select
{
    public class CreateSelectRequest
    {
        public CreateSelectRequest()
        {
            Options = new List<SelectOption>();
        }

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IList<SelectOption> Options { get; set; }
        public SelectType Type { get; set; }
        public int ParentId { get; set; }
        public int ParentOptionId { get; set; }
        public bool IsDashBoard { get; set; }
        public bool IsDer { get; set; }
        public class SelectOption
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }
    }
}
