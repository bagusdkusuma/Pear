using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyOperationData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Scenario Scenario { get; set; }
        public KeyOperationConfig KeyOperationConfig { get; set; }
        public Kpi Kpi { get; set; }
        public double? Value { get; set; }
        public string Remark { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
    }
}
