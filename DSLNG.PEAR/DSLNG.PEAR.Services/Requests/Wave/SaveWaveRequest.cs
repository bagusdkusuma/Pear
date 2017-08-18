using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Periode;

namespace DSLNG.PEAR.Services.Requests.Wave
{
    public class SaveWaveRequest : BaseRequest
    {
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public int ValueId { get; set; }
        public string Tide { get; set; }
        public string Speed { get; set; }
    }
}
