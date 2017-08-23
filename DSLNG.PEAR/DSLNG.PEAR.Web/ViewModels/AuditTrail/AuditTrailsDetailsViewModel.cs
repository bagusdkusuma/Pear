using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DSLNG.PEAR.Web.ViewModels.AuditTrail
{
    public class AuditTrailsDetailsViewModel
    {
        public IList<AuditTrail> AuditTrails { get; set; }

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
            public IList<AuditDelta> OldValue { get; set; }
            public IList<AuditDelta> NewValue { get; set; }

            public class AuditDelta
            {
                public string FieldName { get; set; }
                public string Value { get; set; }
            }
        }
    }
}