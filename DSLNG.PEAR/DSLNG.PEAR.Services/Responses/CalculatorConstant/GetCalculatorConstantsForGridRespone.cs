using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.CalculatorConstant
{
    public class GetCalculatorConstantsForGridRespone 
    {
        public IList<CalculatorConstantsForGrid> CalculatorConstantsForGrids { get; set; }
        public int TotalRecords { get; set; }
        public class CalculatorConstantsForGrid
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public double Value { get; set; }
            public string Measurement { get; set; }
        }
    }
}
