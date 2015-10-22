
using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Web.ViewModels.NLS
{
    public class NLSViewModel
    {
        public int Id { get; set; }
        public int VesselScheduleId { get; set; }
        public string VesselName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
    }
}