using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDersResponse : BaseResponse
    {
        public List<GetDerResponse> Ders { get; set; }
    }
}
