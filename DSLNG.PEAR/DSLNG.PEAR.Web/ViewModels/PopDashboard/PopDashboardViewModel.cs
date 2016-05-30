using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.PopDashboard
{
    public class PopDashboardViewModel
    {
        public PopDashboardViewModel()
        {
            PopDashboards = new List<PopDashboard>();
        }
        public IList<PopDashboard> PopDashboards { get; set; }

        public class PopDashboard
        {
            public int Id { get; set; }
            [Display(Name = "Project Title")]
            public string Title { get; set; }
            [Display(Name="Project No")]
            public string Number { get; set; }
            [Display(Name = "Project Purpose/Objective")]
            public string DashboardObjective { get; set; }
            [Display(Name = "Project Owner(dept/div)")]
            public string StructureOwner { get; set; }
            [Display(Name = "Project Team")]
            public string Team { get; set; }
            [Display(Name = "Budget-OPEX(USD)")]
            public double BudgetOpex { get; set; }
            [Display(Name = "Budget-CAPEX(USD)")]
            public double BudgetCapex { get; set; }
            [Display(Name = "Affected KPI")]
            public string AffectedKPI { get; set; }
            [Display(Name = "Project Start")]
            public DateTime ProjectStart { get; set; }
            [Display(Name = "Project End")]
            public DateTime ProjectEnd { get; set; }
            public string Status { get; set; }

        }
    }


    public class SavePopDashboardViewModel
    {
        public SavePopDashboardViewModel()
        {
            //IsActive = true;
            StatusOptions = new List<SelectListItem>();
            Attachments = new List<AttachmentViewModel>()
            {
                new AttachmentViewModel()
            };
            ProjectStartDispay = "DD/MM/YYYY";
            ProjectEndDispay = "DD/MM/YYYY";
        }
        public int Id { get; set; }
        [Display(Name = "Project Title")]
        [DataType(DataType.MultilineText)]
        [Required]
        [AllowHtml]
        public string Title { get; set; }
        [Display(Name = "Project No")]
        [Required]
        public string Number { get; set; }
        [Display(Name = "Project Purpose/Objective")]
        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DashboardObjective { get; set; }
        [Display(Name = "Project Owner(dept/div)")]
        [Required]
        public string StructureOwner { get; set; }
        [Display(Name = "Project Team")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [Required]
        public string Team { get; set; }
        [Display(Name = "Budget-OPEX(USD)")]
        [Required]
        public double BudgetOpex { get; set; }
        [Display(Name = "Budget-CAPEX(USD)")]
        [Required]
        public double BudgetCapex { get; set; }
        [Display(Name = "Affected KPI")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [Required]
        public string AffectedKPI { get; set; }
        public DateTime? ProjectStart { get; set; }
        public string _projectStartDisplay { get; set; }
        [Display(Name = "Project Start")]
        [Required]
        public string ProjectStartDispay
        {
            get
            {
                if (ProjectStart.HasValue)
                {
                    return ProjectStart.Value.ToString("dd/MM/yyyy");
                }
                return this._projectStartDisplay;
            }
            set
            {
                if (value != "DD/MM/YYYY")
                {
                    this.ProjectStart = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                this._projectStartDisplay = value;
            }
        }

        
        public DateTime? ProjectEnd { get; set; }
        public string _projectEndDisplay { get; set; }
        [Display(Name = "Project End")]
        [Required]
        public string ProjectEndDispay
        {
            get
            {
                if (ProjectEnd.HasValue)
                {
                    return ProjectEnd.Value.ToString("dd/MM/yyyy");
                }
                return this._projectEndDisplay;
            }
            set
            {
                if (value != "DD/MM/YYYY")
                {
                    this.ProjectEnd = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                this._projectEndDisplay = value;
            }
        }
         [Required]
        public string Status { get; set; }

        public IList<SelectListItem> StatusOptions { get; set; }
        public IList<AttachmentViewModel> Attachments { get; set; }

        public class AttachmentViewModel {
            public int Id { get; set; }
            public string Alias { get; set; }
            public string Type { get; set; }
            public string Filename { get; set; }
            public HttpPostedFileBase File { get; set; }
        }
    }

    public class GetPopDashboardViewModel
    {
        public GetPopDashboardViewModel()
        {
            PopInformations = new List<PopInformation>();
            Signatures = new List<SignatureViewModel>();
            IsApprove = false;
            IsReject = false;
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
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string Attachment { get; set; }
        public string Status { get; set; }
        public string DashboardObjective { get; set; }
        public string StructureLeader { get; set; }
        public string StructureOwner { get; set; }
        public double ResourceTotalCost { get; set; }
        public string ResourceCategory { get; set; }
        public string ResourceRemark { get; set; }

        public class PopInformation
        {
            public int Id { get; set; }
            public PopInformationType Type { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
        }
    }
}