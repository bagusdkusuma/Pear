using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Responses.Kpi;

namespace DSLNG.PEAR.Services.Responses.Conversion
{
    public class GetConversionResponse : BaseResponse
    {
        public int Id { get; set; }
        public Measurement From { get; set; }
        public Measurement To { get; set; }
        public float Value { get; set; }
        public string Name { get; set; }
        public bool IsReverse { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetConversionsResponse : BaseResponse
    {
        public IList<Conversion> Conversions { get; set; }
        public int TotalRecords { get; set; }
        public class Conversion
        {
            public int Id { get; set; }
            public Measurement From { get; set; }
            public Measurement To { get; set; }
            public string FromName { get; set; }
            public string ToName { get; set; }
            public float Value { get; set; }
            public string Name { get; set; }
            public bool IsReverse { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
