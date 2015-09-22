using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public FeedGasConstanta FeedGas { get; set; }
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
        }
    }
}