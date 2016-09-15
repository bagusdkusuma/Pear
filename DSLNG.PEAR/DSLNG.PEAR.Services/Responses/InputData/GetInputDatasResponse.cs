using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.InputData
{
    public class GetInputDatasResponse : BaseResponse
    {
        public IList<InputData> InputDatas { get; set; }
        public int TotalRecords { get; set; }
        public class InputData
        {
            public int Id { get; set; }
            public string PeriodeType { get; set; }
            public string Accountability { get; set; }
            public string Name { get; set; }
            public DateTime LastInput { get; set; }
        }
        
    }
}
