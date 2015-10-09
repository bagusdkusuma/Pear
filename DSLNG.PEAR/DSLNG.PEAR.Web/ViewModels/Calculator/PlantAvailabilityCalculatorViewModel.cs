using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class PlantAvailabilityCalculatorViewModel
    {
        public PlantAvailabilityCalculatorViewModel()
        {
            Units = new List<SelectListItem>
                {
                    new SelectListItem {Text = "days", Value = "days"},
                    new SelectListItem {Text = "%", Value = "%"}
                };
        }

        public string Unit { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public int Year { get; set; }
        public int PlantAvailable { get; set; }
        public string PlantAvailability { get; set; }
        public string ShutDown { get; set; }
    }
}