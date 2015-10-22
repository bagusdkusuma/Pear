
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Requests.ConstantUsage;
using DSLNG.PEAR.Web.ViewModels.ConstantUsage;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ConstantUsageController : BaseController
    {
        private IConstantUsageService _constantUsageService;
        private ICalculatorConstantService _calculatorConstantService;
        public ConstantUsageController(IConstantUsageService constantUsageService,
            ICalculatorConstantService calculatorConstantServcie) {
            _constantUsageService = constantUsageService;
            _calculatorConstantService = calculatorConstantServcie;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridConstantUsageIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_IndexGridPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Role");
            viewModel.Columns.Add("Group");
            viewModel.Columns.Add("ConstantNames");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridConstantUsageIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _constantUsageService.GetConstantUsages(new GetConstantUsagesRequest{ OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data =  _constantUsageService.GetConstantUsages(new GetConstantUsagesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).ConstantUsages;
        }

        public ActionResult CalculatorConstants(string term) {
            var results = _calculatorConstantService.GetCalculatorConstants(new GetCalculatorConstantsRequest { Take = 20, Term = term });
            return Json(new { results = results.CalculatorConstants }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ConstantUsageViewModel viewModel) {
            var req = viewModel.MapTo<SaveConstantUsageRequest>();
            _constantUsageService.SaveConstantUsage(req);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var viewModel = _constantUsageService.GetConstantUsage(new GetConstantUsageRequest { Id = id }).MapTo<ConstantUsageViewModel>();
            viewModel.Constants.Insert(0, new ConstantUsageViewModel.CalculatorConstantViewModel());
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Edit(ConstantUsageViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveConstantUsageRequest>();
            _constantUsageService.SaveConstantUsage(req);
            return RedirectToAction("Index");
        }

        //
        // GET: /ConstantUsage/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ConstantUsage/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
