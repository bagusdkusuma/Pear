using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class CreateDerInputFileRequest : BaseRequest
    {
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public int CreatedBy { get; set; }
    }
}
