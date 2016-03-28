using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDafwcDataResponse : BaseResponse
    {
        public string DaysWithoutDafwc { get; set; }
        public string DaysWithoutLopc { get; set; }
        public string DaysWithoutDafwcSince { get; set; }
        public string DaysWithoutLopcSince { get; set; }
    }
}
