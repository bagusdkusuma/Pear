

using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class GetTabularDataResponse : BaseResponse
    {
        public GetTabularDataResponse()
        {
            Rows = new List<RowResponse>();
        }
        public string Title { get; set; }
        public bool Actual { get; set; }
        public bool Target { get; set; }
        public bool Economic { get; set; }
        public bool Fullfillment { get; set; }
        public bool Remark { get; set; }

        public IList<RowResponse> Rows { get; set; }
        public class RowResponse
        {
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Measurement { get; set; }
            public string PeriodeType { get; set; }
            public string Periode { get; set; }
            public double? Actual { get; set; }
            public double? Target { get; set; }
            public string Remark { get; set; }
            public DateTime PeriodeDateTime { get; set; }
        }
    }
}
