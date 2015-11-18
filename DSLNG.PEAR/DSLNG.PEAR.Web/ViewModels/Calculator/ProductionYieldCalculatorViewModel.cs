

using System.ComponentModel.DataAnnotations;
using DSLNG.PEAR.Web.ViewModels.CalculatorConstant;
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.ViewModels.Calculator
{
    public class ProductionYieldCalculatorViewModel
    {
        public ProductionYieldCalculatorViewModel()
        {
            Units = new List<SelectListItem>
            {
                new SelectListItem{Text = "tonnes", Value="tonnes"},
                new SelectListItem{Text = "mmscf", Value="mmscf"},
                new SelectListItem{Text = "kg", Value = "kg"},
                new SelectListItem{Text = "mmbtu", Value = "mmbtu"}
            };
        }
        [Display(Name = "Feed Gas")]
        public double MainInput { get; set; }
        public string Unit { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public LNGViewModel LNG { get; set; }
        public CDSViewModel CDS { get; set; }
        public MCHEViewModel MCHE { get; set; }
        public class LNGViewModel
        {
            public double Tonnes { get; set; }
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
        public class MCHEViewModel
        {
            public double M3PerHr { get; set; }
        }
        public IList<ConstantUsageViewModel> ConstantUsages { get; set; }

        public IList<CalculatorConstantViewModel> Constants
        {
            get
            {
                var data = ConstantUsages.FirstOrDefault(x => x.Role == "production-yield" && x.Group == "all");
                if (data != null)
                {
                    return data.Constants.MapTo<CalculatorConstantViewModel>();    
                }

                return new List<CalculatorConstantViewModel>();
                
            }
        }

        public IList<CalculatorConstantViewModel> GeneralConstans
        {
            get
            {
                var data = ConstantUsages.FirstOrDefault(x => x.Role == "production-yield" && x.Group == "general");
                if (data != null)
                {
                    return data.Constants.MapTo<CalculatorConstantViewModel>();
                }

                return new List<CalculatorConstantViewModel>();
            }
        }
        public IList<CalculatorConstantViewModel> LNGConstants
        {
            get
            {
                var data = ConstantUsages.FirstOrDefault(x => x.Role == "production-yield" && x.Group == "lng");
                if (data != null)
                {
                    return data.Constants.MapTo<CalculatorConstantViewModel>();
                }

                return new List<CalculatorConstantViewModel>();
            }
        }
        public IList<CalculatorConstantViewModel> CDSConstants
        {
            get
            {
                var data = ConstantUsages.FirstOrDefault(x => x.Role == "production-yield" && x.Group == "cds");
                if (data != null)
                {
                    return data.Constants.MapTo<CalculatorConstantViewModel>();
                }

                return new List<CalculatorConstantViewModel>();
            }
        }
        public IList<CalculatorConstantViewModel> MCHEConstants
        {
            get
            {
                var data = ConstantUsages.FirstOrDefault(x => x.Role == "production-yield" && x.Group == "mche");
                if (data != null)
                {
                    return data.Constants.MapTo<CalculatorConstantViewModel>();
                }

                return new List<CalculatorConstantViewModel>();
            }
        }
    }
}