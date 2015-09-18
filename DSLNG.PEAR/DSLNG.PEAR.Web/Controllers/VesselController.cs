using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Web.ViewModels.Vessel;
using System.Web.Mvc;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class VesselController : BaseController
    {
        private IVesselService _vesselService;
        private IMeasurementService _measurementService;

        public VesselController(IVesselService vesselService, IMeasurementService measurementService) {
            _vesselService = vesselService;
            _measurementService = measurementService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselIndex");
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
            viewModel.Columns.Add("Capacity");
            viewModel.Columns.Add("Type");
            viewModel.Columns.Add("Measurement");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _vesselService.GetVessels(new GetVesselsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Vessels;
        }

        //
        // GET: /Vessel/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Vessel/Create
        public ActionResult Create()
        {
            var viewModel = new VesselViewModel();
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return View(viewModel);
        }

        //
        // POST: /Vessel/Create
        [HttpPost]
        public ActionResult Create(VesselViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveVesselRequest>();
            _vesselService.SaveVessel(req);
            return RedirectToAction("Index");
        }

        //
        // GET: /Vessel/Edit/5
        public ActionResult Edit(int id)
        {
            var vessel = _vesselService.GetVessel(new GetVesselRequest { Id = id });
            var viewModel = vessel.MapTo<VesselViewModel>();
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return View(viewModel);
        }

        //
        // POST: /Vessel/Edit/5
        [HttpPost]
        public ActionResult Edit(VesselViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveVesselRequest>();
            _vesselService.SaveVessel(req);
            return RedirectToAction("Index");
        }

        //
        // GET: /Vessel/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Vessel/Delete/5
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
