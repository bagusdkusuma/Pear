using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Wave
{
     public class GetWaveRequest
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public bool ByDate { get; set; }
    }
}
