using System;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class DeleteFilenameRequest : BaseRequest
    {
        public string filename { get; set; }
        public DateTime date { get; set; }
    }
}
