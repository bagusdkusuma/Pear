using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.InputOriginalData
{
    public class InputData : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RoleGroup Accountability { get; set; }
        public string Name { get; set; }
        public DateTime LastInput { get; set; }
        public IList<GroupInputData> GroupInputDatas { get; set; }
    }
}
