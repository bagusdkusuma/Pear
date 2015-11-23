using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Level;
using DSLNG.PEAR.Web.ViewModels.Level;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class LevelController : BaseController
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }
        //
        // GET: /Level/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridLevelIndex");

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
            viewModel.Columns.Add("Code");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Number");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridLevelIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _levelService.GetLevels(new GetLevelsRequest()).Levels.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _levelService.GetLevels(new GetLevelsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Levels;
        }

        public ActionResult Create() {
            var viewModel = new CreateLevelViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateLevelViewModel viewModel)
        { 
            var request  = viewModel.MapTo<CreateLevelRequest>();
            var response = _levelService.Create(request);
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
            var response = _levelService.GetLevel(new GetLevelRequest { Id = id });
            var viewModel = response.MapTo<UpdateLevelViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateLevelViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateLevelRequest>();
            var response = _levelService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Update", viewModel);
        }

        public ActionResult Delete(int id) {
            var response = _levelService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var level = _levelService.GetLevels(new GetLevelsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = level.TotalRecords,
                iTotalRecords = level.Levels.Count,
                aaData = level.Levels
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

	}
}