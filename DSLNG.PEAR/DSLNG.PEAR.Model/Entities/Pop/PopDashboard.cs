using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Pop
{
    public class PopDashboard
    {
        public PopDashboard() {
            Attachments = new List<PopDashboardAttachment>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string DashboardObjective { get; set; }
        public string StructureOwner { get; set; }
        public string Team { get; set; }
        public double BudgetOpex { get; set; }
        public double BudgetCapex { get; set; }
        public string AffectedKPI { get; set; }
        public DateTime ProjectStart { get; set; }
        public DateTime ProjectEnd { get; set; }
        public string Status { get; set; }
        public IList<PopDashboardAttachment> Attachments { get; set; }

        public IList<PopInformation> PopInformations { get; set; }
        public IList<Signature> Signatures { get; set; }
        public string Attachment { get; set; }
    }
}
