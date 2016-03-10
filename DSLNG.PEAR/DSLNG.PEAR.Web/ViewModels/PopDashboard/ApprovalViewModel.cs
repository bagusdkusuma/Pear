using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.PopDashboard
{
    public class ApprovalViewModel
    {
        public int Id { get; set; }
        public PopInformationInApprovalViewModel PopInformation { get; set; }

        public class PopInformationInApprovalViewModel
        {
            public PopInformationInApprovalViewModel()
            {
                PopInformations = new List<PopInformation>();
                Signatures = new List<SignatureViewModel>();
            }

            public int Id { get; set; }
            public string Title { get; set; }
            public string Number { get; set; }
            public string Subtitle { get; set; }
            public bool IsActive { get; set; }
            public IList<PopInformation> PopInformations { get; set; }
            public IList<SignatureViewModel> Signatures { get; set; }
            public List<SelectListItem> Users { get; set; }
            public int UserId { get; set; }
            public int DashboardId { get; set; }
            public int TypeSignature { get; set; }

            public class PopInformation
            {
                public int Id { get; set; }
                public PopInformationType Type { get; set; }
                public string Title { get; set; }
                public string Value { get; set; }
            }
        }
    }
}