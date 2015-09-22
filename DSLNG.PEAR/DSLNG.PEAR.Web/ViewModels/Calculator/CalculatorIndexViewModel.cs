using DSLNG.PEAR.Web.ViewModels.CalculatorConstant;
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;
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
            ProductionYield = new ProductionYieldCalculatorViewModel();
            Pricing = new PricingViewModel();
        }
        public IList<ConstantUsageViewModel> ConstantUsages { set {
            ProductionYield.ConstantUsages = value;
        } }

        public ProductionYieldCalculatorViewModel ProductionYield { get; set; }
        public PricingViewModel Pricing { get; set; }
    }
}