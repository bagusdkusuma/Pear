using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiTransformation
{
    public class DeleteTransformationRequest :BaseRequest
    {
        public int Id { get; set; }
    }
}
