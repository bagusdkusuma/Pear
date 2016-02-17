using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class EnvironmentalScanning
    {
        [Key]
        public int Id { get; set; }
        public string Desc { get; set; }
        public EnvironmentsScanning ThreatHost { get; set; }
        public EnvironmentsScanning OpportunityHost { get; set; }
        public EnvironmentsScanning WeaknessHost { get; set; }
        public EnvironmentsScanning StrengthHost { get; set; }
        public IList<Constraint> Constraints { get; set; }
        public IList<Challenge> Challenges { get; set; }

    }
}
