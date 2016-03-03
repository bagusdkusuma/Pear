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
    }
}