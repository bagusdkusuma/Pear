using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace DSLNG.PEAR.Services.Responses.AuditTrail
{
    public class AuditTrailsResponse : BaseResponse
    {
        public IList<AuditTrail> AuditTrails { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }

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
