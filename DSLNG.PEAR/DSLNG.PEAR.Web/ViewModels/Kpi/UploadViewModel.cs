using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Kpi
{
	public class UploadViewModel
	{
        [DataType(DataType.Upload)]
        public HttpPostedFileBase IconFile { get; set; }
        public int KpiId { get; set; }
	}
}