using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.AuditTrail
{
    public class GetAuditTrailViewModel
    {
        public GetAuditTrailViewModel()
        {
            AuditTrails = new List<AuditTrail>();
            YearList = new List<SelectListItem>();
        }
        public IList<AuditTrail> AuditTrails { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
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
                list.Insert(0, new SelectListItem { Value = "0", Text = "All Month" });
                return list;
            }
        }
        public IList<SelectListItem> YearList { get; set; }

        public class AuditTrail
        {
            public int Id { get; set; }
            public DateTime UpdateDate { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public string Action { get; set; }
            public int RecordId { get; set; }
            public string TableName { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }
    }
}