

using DSLNG.PEAR.Data.Enums;
using System;
namespace DSLNG.PEAR.Services.Requests.Highlight
{
    public class GetHighlightRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public int HighlightTypeId { get; set; }
        public PeriodeType? PeriodeType { get; set; }
    }
}
