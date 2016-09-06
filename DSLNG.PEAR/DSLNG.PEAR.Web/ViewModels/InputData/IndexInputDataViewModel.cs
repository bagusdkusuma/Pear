using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.InputData
{
    public class IndexInputDataViewModel
    {
        public IndexInputDataViewModel()
        {
            InputDatas = new List<InputDataViewModel>();
        }
        public IList<InputDataViewModel> InputDatas { get; set; }

        public class InputDataViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Accountability { get; set; }
            public string LastInput { get; set; }
            public string PeriodeType { get; set; }
        }
    }

    
}