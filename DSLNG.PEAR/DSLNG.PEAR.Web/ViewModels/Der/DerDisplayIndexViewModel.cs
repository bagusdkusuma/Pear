using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class DerDisplayViewModel
    {
        public DerDisplayViewModel()
        {
            Items = new List<DerLayoutItem>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public string Today
        {
            get { return DateTime.Now.ToShortDateString(); }
        }

        public IList<DerLayoutItem> Items { get; set; }

        public class DerLayoutItem
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public DerArtifact Artifact { get; set; }
        }

        public class DerArtifact
        {
            public DerArtifact()
            {
                Series = new List<DerArtifactSerie>();
            }
            public int Id { get; set; }
            public string HeaderTitle { get; set; }
            public int MeasurementId { get; set; }
            public string MeasurementName { get; set; }
            public string GraphicType { get; set; }

            public IList<DerArtifactSerie> Series { get; set; }
        }

        public class DerArtifactSerie
        {
            public int Id { get; set; }
            public string Label { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Color { get; set; }
        }
    }
}