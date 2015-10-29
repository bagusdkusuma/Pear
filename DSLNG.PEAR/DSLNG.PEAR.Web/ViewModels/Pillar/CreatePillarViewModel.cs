using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSLNG.PEAR.Web.ViewModels.Kpi;

namespace DSLNG.PEAR.Web.ViewModels.Pillar
{
    public class CreatePillarViewModel
    {
        public CreatePillarViewModel()
        {
            Upload = new UploadViewModel();
            Icons = new List<string>();
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public UploadViewModel Upload { get; set; }
        public IList<string> Icons { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}