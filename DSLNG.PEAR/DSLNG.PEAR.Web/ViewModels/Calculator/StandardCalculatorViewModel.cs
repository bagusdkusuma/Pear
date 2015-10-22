
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class StandardCalculatorViewModel
    {
        public StandardCalculatorViewModel() {
            UnitGroups = new List<SelectListItem> { 
                new SelectListItem{Value = "weight", Text="Weight"},
                new SelectListItem{Value = "thermal", Text="Thermal"},
                new SelectListItem{Value = "volume", Text="Volume"},
                new SelectListItem{Value = "length", Text="Length"},
                new SelectListItem{Value = "pressure", Text="Pressure"},
                new SelectListItem{Value = "density", Text="Density"},
                new SelectListItem{Value = "temperature", Text="Temperature"},
                new SelectListItem{Value = "storage", Text="Storage"},
            };
        }
        public double Input { get; set; }
        public double Output { get; set; }
        public IList<SelectListItem> UnitGroups { get; set; }
        public string InputUnitGroup { get; set; }
        public string OutputUnitGroup { get; set; }
        public IList<ConstantUsageViewModel> ConstantUsages { get; set; }
    }
}