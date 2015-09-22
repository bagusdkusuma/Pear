using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Calculator;

namespace DSLNG.PEAR.Web.Controllers
{
    public class CalculatorController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new CalculatorIndexViewModel();
            return View(viewModel);
        }
	}
}