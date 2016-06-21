using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerLayoutItem
    {
        public DerLayoutItem()
        {
            KpiInformations = new Collection<DerKpiInformation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public DerLayout DerLayout { get; set; }
        public DerArtifact Artifact { get; set; }
        public DerHighlight Highlight { get; set; }
        public DerStaticHighlight StaticHighlight { get; set; }
        public ICollection<DerKpiInformation> KpiInformations { get; set; }
        public ICollection<DerOriginalData> OriginalData { get; set; } 
        public User SignedBy { get; set; }
    }
}
