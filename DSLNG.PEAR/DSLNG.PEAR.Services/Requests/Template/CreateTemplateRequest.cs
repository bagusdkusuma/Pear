

using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.Template
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; }
        public int RefershTime { get; set; } //in minutes
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public IList<RowRequest> LayoutRows { get; set; }

        public class RowRequest
        {
            public int Index { get; set; }
            public IList<ColumnRequest> LayoutColumns { get; set; }
        }

        public class ColumnRequest
        {
            public int Index { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public int ArtifactId { get; set; }
            public int HighlightTypeId { get; set; }
            public PeriodeType HighlightPeriodeType { get; set; }
            public TemplateColumnType ColumnType { get; set; }
        }
    }
}
