using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class Constraint
    {
        public Constraint()
        {
            Relations = new List<EnvironmentalScanning>();
        }
        [Key]
        public int Id { get; set; }
        public IList<EnvironmentalScanning> Relations { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public ESCategory ESCategory { get; set; }
        public EnvironmentsScanning EnvironmentScanning { get; set; }
    }
}
