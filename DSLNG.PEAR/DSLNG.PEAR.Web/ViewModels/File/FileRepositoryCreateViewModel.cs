using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.File
{
    public class FileRepositoryCreateViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public string Name { get; set; }
        [AllowHtml]
        public string Summary { get; set; }
        [Display(Name="Ex. Summary Default Page"),Required]
        [UIHint("number")]
        public int ExSumDefaultPage { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }

        public DateTime? LastWriteTime { get; set; }
    }
}
