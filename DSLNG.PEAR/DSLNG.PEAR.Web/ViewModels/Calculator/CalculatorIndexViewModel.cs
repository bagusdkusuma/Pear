using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class CalculatorIndexViewModel
    {
        public CalculatorIndexViewModel()
        {
            Units = new List<SelectListItem>();
            ProductionYield = new ProductionYieldCalculatorViewModel();
            Pricing = new PricingViewModel();
        }
        public int Number { get; set; }
        public string Unit { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public ProductionYieldCalculatorViewModel ProductionYield { get; set; }
        public PricingViewModel Pricing { get; set; }
    }
}