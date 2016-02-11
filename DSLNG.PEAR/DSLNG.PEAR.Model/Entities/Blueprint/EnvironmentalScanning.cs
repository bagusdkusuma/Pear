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
        public EnvironmentsScanning Threat { get; set; }
        public EnvironmentsScanning Opportunity { get; set; }
        public EnvironmentsScanning Weakness { get; set; }
        public EnvironmentsScanning Strength { get; set; }
    }
}
