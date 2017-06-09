

using System.Collections.Generic;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Template
{
    public class TemplateViewModel
    {
        public TemplateViewModel()
        {
            PeriodeTypes = new List<SelectListItem>();
            HighlightTypes = new List<SelectListItem>();
            ColumnTypes = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int RefershTime { get; set; } //in minutes
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public IList<RowViewModel> LayoutRows { get; set; }
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public IList<SelectListItem> HighlightTypes { get; set; }
        public IList<SelectListItem> ColumnTypes { get; set; }
        public class RowViewModel
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public IList<ColumnViewModel> LayoutColumns { get; set; }
        }

        public class ColumnViewModel
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public int ArtifactId { get; set; }
            public string ArtifactName { get; set; }
            public int HighlightTypeId { get; set; }
            public string HighlightPeriodeType { get; set; }
            public string ColumnType { get; set; }
        }
    }
}