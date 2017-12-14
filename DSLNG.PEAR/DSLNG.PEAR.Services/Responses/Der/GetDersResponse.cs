using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDersResponse : BaseResponse
    {
        public GetDersResponse()
        {
            Ders = new List<Der>();
        }
        public List<Der> Ders { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class Der
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public DateTime Date { get; set; }
            public int Revision { get; set; }
            public string Filename { get; set; }
            public string GenerateBy { get; set; }
            public string RevisionBy { get; set; }
            public bool IsActive { get; set; }
            public string OverallPerformance { get; set; }
            public string MarineCargoDelivery { get; set; }
            public string DailyIndicator { get; set; }
            public string Qhse { get; set; }
            public string Security { get; set; }
            public string PlantMode { get; set; }
        }
    }
}
