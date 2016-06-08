using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Kpi
{
    public class UpdateKpiViewModel
    {
        public UpdateKpiViewModel()
        {
            RelationModels = new List<KpiRelationModel>()
                {
                    new KpiRelationModel()
                };
            Upload = new UploadViewModel();
            Icons = new List<string>();
        }
        public string CodeFromLevel { get; set; }
        public string CodeFromPillar { get; set; }
        public string CodeFromRoleGroup { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "KPI Code")]
        public string Code { get; set; }

        [Required]
        [Display(Name="KPI Name")]
        public string Name { get; set; }

        [Display(Name="Pillar")]
        public int? PillarId { get; set; } //to make this nullable we need to include it
        //public Pillar Pillar { get; set; }
        public IEnumerable<SelectListItem> PillarList { get; set; }

        [Required]
        [Display(Name = "Level")]
        public int LevelId { get; set; }
        public IEnumerable<SelectListItem> LevelList { get; set; }
        //public Level Level { get; set; }

        [Required]
        [Display(Name = "Accountability")]
        public int? RoleGroupId { get; set; }
        public IEnumerable<SelectListItem> RoleGroupList { get; set; }
        //public RoleGroup RoleGroup { get; set; } 

        [Required]
        [Display(Name = "Type")]
        public int TypeId { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }
        //public Type Type { get; set; }

        [Display(Name = "Group")]
        public int? GroupId { get; set; }
        public IEnumerable<SelectListItem> GroupList { get; set; }
        //public Group Group { get; set; }

        [Display(Name = "Is Economic Model")]
        public bool IsEconomic { get; set; }

        [Required]
        [Display(Name = "Ordering")]
        public int Order { get; set; }

        public YtdFormula YtdFormula { get; set; }
        [Required]
        [Display(Name = "Aggregation Formula")]
        public string YtdFormulaValue { get; set; }
        public IEnumerable<SelectListItem> YtdFormulaList { get; set; }


        [Display(Name = "Custom Formula")]
        public string CustomFormula { get; set; }

        [Display(Name="Measurement")]
        [Required]
        public int? MeasurementId { get; set; }
        public IEnumerable<SelectListItem> MeasurementList { get; set; }
        //public Measurement Measurement { get; set; }

        [Required]
        [Display(Name = "Method Value")]
        public int MethodId { get; set; }
        public IEnumerable<SelectListItem> MethodList { get; set; }
        //public Method Method { get; set; }

        public int? ConversionId { get; set; }
        public Conversion Conversion { get; set; }
        public FormatInput FormatInput { get; set; }

        //public Periode Periode { get; set; }
        [Required]
        [Display(Name = "Period Input")]
        public string PeriodeValue { get; set; }
        public IEnumerable<SelectListItem> PeriodeList { get; set; }
        public PeriodeType Periode { get; set; }

        public string Remark { get; set; }

        public ICollection<KpiRelationModel> RelationModels { get; set; }
        public IEnumerable<SelectListItem> KpiList { get; set; }
        public DateTime? Value { get; set; }

        
        public string Icon { get; set; }
        public IList<string> Icons { get; set; }
        public UploadViewModel Upload { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}