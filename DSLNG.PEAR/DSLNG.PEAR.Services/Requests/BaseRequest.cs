using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests
{
    public class BaseRequest
    {
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
