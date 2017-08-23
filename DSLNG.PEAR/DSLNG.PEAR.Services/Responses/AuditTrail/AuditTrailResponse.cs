using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AuditTrail
{
    public class AuditTrailResponse
    {
        public int Id { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Action { get; set; }
        public int RecordId { get; set; }
        public string TableName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public class User
        {
            public int Id { get; set; }
            public string UserName { get; set; }
        }
    }
}
