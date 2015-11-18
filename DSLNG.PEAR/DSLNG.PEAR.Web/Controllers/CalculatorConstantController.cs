
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Web.ViewModels.CalculatorConstant;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class CalculatorConstantController : BaseController
    {
        ICalculatorConstantService _calculatorConstantService;
        public CalculatorConstantController(ICalculatorConstantService calculatorConstantService)
        {
            _calculatorConstantService = calculatorConstantService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridCalConstantIndex");
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
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("DisplayName");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("Measurement");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridCalConstantIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _calculatorConstantService.GetCalculatorConstants(new GetCalculatorConstantsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _calculatorConstantService.GetCalculatorConstants(new GetCalculatorConstantsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).CalculatorConstants;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CalculatorConstantViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveCalculatorConstantRequest>();
            _calculatorConstantService.SaveCalculatorConstant(req);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var viewModel = _calculatorConstantService.GetCalculatorConstant(new GetCalculatorConstantRequest { Id = id })
                .MapTo<CalculatorConstantViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(CalculatorConstantViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                var req = new SaveCalculatorConstantRequest();
                req.Id = viewModel.Id;
                req.Value = viewModel.Value;
                req.IsAjaxRequest = true;
                var response = _calculatorConstantService.SaveCalculatorConstant(req);
                return Json(response);

            }
            else
            {
                var req = viewModel.MapTo<SaveCalculatorConstantRequest>();
                _calculatorConstantService.SaveCalculatorConstant(req);
                return RedirectToAction("Index");    
            }
            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _calculatorConstantService.DeleteCalculatorConstant(new DeleteCalculatorConstantRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var calculator = _calculatorConstantService.GetCalculatorConstantsForGrid(new GetCalculatorConstantForGridRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = calculator.TotalRecords,
                iTotalRecords = calculator.CalculatorConstantsForGrids.Count,
                aaData = calculator.CalculatorConstantsForGrids

            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}