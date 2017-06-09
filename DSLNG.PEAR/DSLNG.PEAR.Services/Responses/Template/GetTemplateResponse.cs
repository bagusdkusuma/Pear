

using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Template
{
    public class GetTemplateResponse
    {
        public GetTemplateResponse(){
            LayoutRows = new List<RowResponse>();
        }
        public string Name { get; set; }
        public int RefershTime { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }

        public IList<RowResponse> LayoutRows{get;set;}


        public class RowResponse {
            public RowResponse() {
                LayoutColumns = new List<ColumnResponse>();
            }
            public int Id { get; set; }
            public int Index { get; set; }
            public IList<ColumnResponse> LayoutColumns { get; set; }
        }

        public class ColumnResponse {
            public int Id { get; set; }
            public int Index { get; set; }
            public double Width { get; set; }
            public string ArtifactName { get; set; }
            public int ArtifactId { get; set; }
            public TemplateColumnType ColumnType { get; set; }
            public PeriodeType HighlightPeriodeType { get; set; }
            public int HighlightTypeId { get; set; }
        }
    }
}
