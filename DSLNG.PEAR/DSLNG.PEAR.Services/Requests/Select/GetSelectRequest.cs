using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Select
{
    public class GetSelectRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public int ParentOptionId { get; set; }
        public bool IsActive { get; set; }
    }
}
