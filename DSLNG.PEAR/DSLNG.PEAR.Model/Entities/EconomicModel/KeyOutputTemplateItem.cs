using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyOutputTemplateItem
    {
        public KeyOutputCategory KeyOutputCategory { get; set; }
        public IList<KeyOutputConfiguration> KeyOutput { get; set; }
    }
}
