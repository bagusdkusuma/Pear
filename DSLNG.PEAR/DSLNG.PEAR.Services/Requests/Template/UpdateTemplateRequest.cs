using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Template
{
    public class UpdateTemplateRequest
    {
        public int Id { get; set; }
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
