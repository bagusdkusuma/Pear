
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Pop
{
    public class PopDashboardAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Filename { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public PopDashboard PopDashboard { get; set; }
    }
}
