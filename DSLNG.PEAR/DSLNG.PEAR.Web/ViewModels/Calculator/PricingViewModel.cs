using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.CalculatorConstant;
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;

namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class PricingViewModel
    {
        public PricingViewModel()
        {
            Units = new List<SelectListItem>
                {
                    new SelectListItem {Text = "USD/bbl", Value = "usd/bbl"}
                };
        }

        public double JccPrice { get; set; }
        public string Unit { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public IList<ConstantUsageViewModel> ConstantUsages { get; set; }
        public IList<CalculatorConstantViewModel> FeedGasConstants
        {
            get
            {
                return ConstantUsages.First(x => x.Role == "pricing" && x.Group == "feed-gas")
                    .Constants.MapTo<CalculatorConstantViewModel>();
            }
        }

        public IList<CalculatorConstantViewModel> OtherConstants
        {
            get
            {
                return ConstantUsages.First(x => x.Role == "pricing" && x.Group == "other")
                    .Constants.MapTo<CalculatorConstantViewModel>();
            }
        }

        public IList<CalculatorConstantViewModel> CompositionConstants
        {
            get
            {
                return ConstantUsages.First(x => x.Role == "pricing" && x.Group == "composition")
                    .Constants.MapTo<CalculatorConstantViewModel>();
            }
        }

        public IList<CalculatorConstantViewModel> CdsPriceConstants
        {
            get
            {
                return ConstantUsages.First(x => x.Role == "pricing" && x.Group == "cds-price")
                    .Constants.MapTo<CalculatorConstantViewModel>();
            }
        }


        /*public FeedGasConstanta FeedGas { get; set; }
        public CdsPriceConstanta CdsPrice { get; set; }
        public LngSpaDesConstanta LngSpaDes { get; set; }
        public CompositionConstanta Composition { get; set; }

        public class FeedGasConstanta
        {
            public double SenoroGsaPrice { get; set; }
            public double MatindokGsaPrice { get; set; }
            public double JccFloor { get; set; }
        }

        public class CdsPriceConstanta
        {
            public double PriceVariable { get; set; }
            public double PriceConstanta { get; set; }
        }

        public class LngSpaFobConstanta
        {
            public double Constanta1 { get; set; }
        }

        public class LngSpaDesConstanta
        {
            public double Constanta1 { get; set; }
        }

        public class CompositionConstanta
        {
            public double Matindok { get; set; }
            public double Senoro { get; set; }
        }*/
    }
}