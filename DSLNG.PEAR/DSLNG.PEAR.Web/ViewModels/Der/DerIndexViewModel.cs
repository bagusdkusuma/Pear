using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class DerIndexViewModel
    {
        public DerIndexViewModel()
        {
            Items = new List<DerItem>();
            YearList = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<DerItem> Items { get; set; }
        public IEnumerable<SelectListItem> MonthList
        {
            get
            {
                var list = DateTimeFormatInfo
                   .InvariantInfo
                   .MonthNames
                   .Where(m => !String.IsNullOrEmpty(m))
                   .Select((monthName, index) => new SelectListItem
                   {
                       Value = (index + 1).ToString(),
                       Text = monthName
                   }).ToList();
                list.Insert(0, new SelectListItem { Value = "0", Text = "All Year" });
                return list;
            }
        }
        public IList<SelectListItem> YearList { get; set; }
        public class DerItem
        {
            public string Type { get; set; } //text, highlight, artifact, image
            public int? ComponentId { get; set; }
            public string Text { get; set; }
            public string FileLocation { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }
    }
}