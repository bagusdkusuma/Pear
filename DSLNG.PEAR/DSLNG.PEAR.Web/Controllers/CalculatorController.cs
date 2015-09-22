
using DSLNG.PEAR.Web.ViewModels.Calculator;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.ConstantUsage;
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;

namespace DSLNG.PEAR.Web.Controllers
{
    public class CalculatorController : BaseController
    {
        private IConstantUsageService _constantUsageService;
        public CalculatorController(IConstantUsageService constantUsageService)
        {
            _constantUsageService = constantUsageService;
        }
        public ActionResult Index()
        {
            var viewModel = new CalculatorIndexViewModel();
            viewModel.ConstantUsages = _constantUsageService.GetConstantUsages(new GetConstantUsagesRequest()).ConstantUsages.MapTo<ConstantUsageViewModel>();
            return View(viewModel);
        }
	}
}