
using DSLNG.PEAR.Data.Enums;
using System;

namespace DSLNG.PEAR.Services.Responses.Highlight
{
    public class GetHighlightResponse
    {
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string SPeriodeType { get { return PeriodeType.ToString();  } }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }
}
