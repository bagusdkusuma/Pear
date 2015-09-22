

using System.Collections.Generic;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class ProductionYieldCalculatorViewModel
    {
        public ProductionYieldCalculatorViewModel() {
            Units = new List<SelectListItem>
            {
                new SelectListItem{Text = "tonnes", Value="tonnes"},
                new SelectListItem{Text = "mmscf", Value="mmscf"},
                new SelectListItem{Text = "kg", Value = "kg"},
                new SelectListItem{Text = "mmbtu", Value = "mmbtu"}
            };
        }
        public double MainInput { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public LNGViewModel LNG { get; set; }
        public CDSViewModel CDS { get; set; }
        public MHCEViewModel MHCE { get; set; }
        public class LNGViewModel {
            public double Tonnes { get; set;}
            public double Mmbtu { get; set; }
            public double M3 { get; set; }
            public double Mtpa { get; set; }
        }
        public class CDSViewModel
        {
            public double Tonnes { get; set; }
            public double Mmbtu { get; set; }
            public double M3 { get; set; }
            public double Bbl { get; set; }
        }
        public class MHCEViewModel
        {
            public double M3PerHr { get; set; }
        }
    }
}