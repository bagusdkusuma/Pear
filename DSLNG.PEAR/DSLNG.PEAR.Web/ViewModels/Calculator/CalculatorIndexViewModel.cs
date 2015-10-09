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
            Pricing = new PricingCalculatorViewModel();
            Standard = new StandardCalculatorViewModel();
            PlantAvailability = new PlantAvailabilityCalculatorViewModel();
        }
        public IList<ConstantUsageViewModel> ConstantUsages
        {
            set
            {
                ProductionYield.ConstantUsages = value;
                Standard.ConstantUsages = value;
                Pricing.ConstantUsages = value;
            }
        }

        public ProductionYieldCalculatorViewModel ProductionYield { get; set; }
        public PricingCalculatorViewModel Pricing { get; set; }
        public StandardCalculatorViewModel Standard { get; set; }
        public PlantAvailabilityCalculatorViewModel PlantAvailability { get; set; }
    }
}