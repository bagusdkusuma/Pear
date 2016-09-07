using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.InputOriginalData
{
    public class GroupInputData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<InputDataKpiAndOrder> InputDataKpiAndOrders {get;set;}
        public int Order { get; set; }
        public InputData InputData { get; set; }
    }
}
