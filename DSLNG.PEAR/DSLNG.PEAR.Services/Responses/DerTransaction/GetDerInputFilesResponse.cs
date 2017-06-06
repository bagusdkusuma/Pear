using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.DerTransaction
{
    public class GetDerInputFilesResponse
    {
        public GetDerInputFilesResponse()
        {
            DerInputFiles = new List<DerInputFile>();
        }
        public List<DerInputFile> DerInputFiles { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }

        public class DerInputFile
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string FileName { get; set; }
            public string Title { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
