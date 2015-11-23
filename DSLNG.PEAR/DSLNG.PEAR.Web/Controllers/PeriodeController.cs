using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Periode;
using DSLNG.PEAR.Services.Responses.Periode;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Periode;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PeriodeController : BaseController
    {
        private readonly IPeriodeService _periodeService;
        public PeriodeController(IPeriodeService service)
        {
            _periodeService = service;
        }
        // GET: Periode
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridPeriodeIndex");

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
            return PartialView("_GridViewPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridPeriodeIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _periodeService.GetPeriodes(new GetPeriodesRequest()).Periodes.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _periodeService.GetPeriodes(new GetPeriodesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Periodes;
        }

        public ActionResult Create()
        {
            var viewModel = new CreatePeriodeViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePeriodeViewModel viewModel)
        {
            var request = viewModel.MapTo<CreatePeriodeRequest>();
            var response = _periodeService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public ActionResult Update(int id)
        {
            var response = _periodeService.GetPeriode(new GetPeriodeRequest { Id = id });
            var viewModel = response.MapTo<UpdatePeriodeViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdatePeriodeViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdatePeriodeRequest>();
            var response = _periodeService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Update", viewModel);
        }

        public ActionResult Delete(int id)
        {
            var response = _periodeService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var periode = _periodeService.GetPeriodesForGrid(new GetPeriodesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = periode.TotalRecords,
                iTotalRecords = periode.Periodes.Count,
                aaData = periode.Periodes.Select(x => new
                {
                    x.Id,
                    x.IsActive,
                    Name = x.Name.ToString(),
                    x.Remark
                })

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}