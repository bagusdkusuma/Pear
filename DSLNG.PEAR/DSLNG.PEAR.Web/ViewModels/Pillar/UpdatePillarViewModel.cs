using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Web.ViewModels.Kpi;

namespace DSLNG.PEAR.Web.ViewModels.Pillar
{
    public class UpdatePillarViewModel
    {
        public UpdatePillarViewModel()
        {
            Icons = new List<string>();
            Upload = new UploadViewModel();
        }
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }

        public string Icon { get; set; }
        public IList<string> Icons { get; set; }
        public UploadViewModel Upload { get; set; }
    }
}