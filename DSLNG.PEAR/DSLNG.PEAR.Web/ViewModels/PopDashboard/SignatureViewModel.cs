using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.PopDashboard
{
    public class SignatureViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public SignatureType Type { get; set; }
        public string SignatureImage { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string Note { get; set; }
    }

    public class SaveApprovalViewModel
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public int UserId { get; set; }
        public SignatureType Type { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string Note { get; set; }
    }
}