using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Type;
using DSLNG.PEAR.Services.Requests.Type;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class TypeController : BaseController
    {
        private readonly ITypeService _typeService;

        public TypeController(ITypeService service)
        {
            _typeService = service;
        }

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridTypeIndex");
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
            var viewModel = GridViewExtension.GetViewModel("gridTypeIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _typeService.GetTypes(new GetTypesRequest()).Types.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _typeService.GetTypes(new GetTypesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Types;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateTypeViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateTypeViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateTypeRequest>();
            var response = _typeService.Create(request);
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
            var response = _typeService.GetType(new GetTypeRequest { Id = id });
            var viewModel = response.MapTo<UpdateTypeViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateTypeViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateTypeRequest>();
            var response = _typeService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _typeService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var type = _typeService.GetTypes(new GetTypesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = type.TotalRecords,
                iTotalRecords = type.Types.Count,
                aaData = type.Types
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}