using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class CreateOrUpdateDerRequest : BaseRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Revision { get; set; }
        public string Filename { get; set; }
        public int RevisionBy { get; set; }
        public bool IsActive { get; set; }
    }
}
