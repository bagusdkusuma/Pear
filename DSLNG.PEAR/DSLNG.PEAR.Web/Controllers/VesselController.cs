using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Vessel;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class VesselController : BaseController
    {
        private IVesselService _vesselService;
        public VesselController(IVesselService vesselService) {
            _vesselService = vesselService;
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
            return View();
        }

        //
        // POST: /Vessel/Create
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
        // GET: /Vessel/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Vessel/Edit/5
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
