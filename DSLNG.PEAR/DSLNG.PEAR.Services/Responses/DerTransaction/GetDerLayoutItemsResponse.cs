using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.DerTransaction
{
    public class GetDerLayoutItemsResponse
    {
        public IList<DerLayoutItem> DerLayoutItems { get; set; }
        public class DerLayoutItem {
            public int Id { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public string Type { get; set; }
            public DerHighlight Highlight { get; set; }
            public IList<KpiInformation> KpiInformations { get; set; }
        }

        public class DerHighlight {
            public int Id { get; set; }
            public string Value { get; set; }
            public string Text { get; set; }
            public int HighlightTypeId { get; set; }
        }

        public class KpiInformation {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public int Position { get; set; }
            public ConfigType ConfigType { get; set; }
        }
    }
}
