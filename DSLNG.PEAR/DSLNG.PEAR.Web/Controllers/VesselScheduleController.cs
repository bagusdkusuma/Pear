using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class VesselScheduleController : Controller
    {
        private readonly IVesselScheduleService _vesselScheduleService;
        public VesselScheduleController(IVesselScheduleService vesselScheduleService) {
            _vesselScheduleService = vesselScheduleService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselScheduleIndex");
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
            viewModel.Columns.Add("Vessel");
            viewModel.Columns.Add("ETA");
            viewModel.Columns.Add("ETD");
            viewModel.Columns.Add("Buyer");
            viewModel.Columns.Add("Location");
            viewModel.Columns.Add("SalesType");
            viewModel.Columns.Add("Type");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselScheduleIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).VesselSchedules;
        }

        //
        // GET: /VesselSchedule/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /VesselSchedule/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VesselSchedule/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VesselSchedule/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /VesselSchedule/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VesselSchedule/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /VesselSchedule/Delete/5
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
