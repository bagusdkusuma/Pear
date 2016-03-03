using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDerItemResponse : BaseResponse
    {
       public int Id { get; set; }
        public string Type { get; set; } //text, highlight, artifact, image
        public int? ComponentId { get; set; }
        public string Text { get; set; }
        public string FileLocation { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
