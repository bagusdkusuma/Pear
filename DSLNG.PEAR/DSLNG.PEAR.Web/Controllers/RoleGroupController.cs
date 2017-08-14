using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.RoleGroup;
using DSLNG.PEAR.Services.Requests.RoleGroup;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class RoleGroupController : BaseController
    {
        private readonly IRoleGroupService _roleGroupService;
        private readonly ILevelService _levelService;

        public RoleGroupController(IRoleGroupService roleService, ILevelService levelService)
        {
            _roleGroupService = roleService;
            _levelService = levelService;
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
            viewModel.Columns.Add("Icon");
            viewModel.Columns.Add("LevelName");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridRoleGroupIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _roleGroupService.GetRoleGroups(new GetRoleGroupsRequest()).RoleGroups.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _roleGroupService.GetRoleGroups(new GetRoleGroupsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).RoleGroups;
        }

        public CreateRoleGroupViewModel CreateViewModel(CreateRoleGroupViewModel viewModel)
        {
            viewModel.LevelList = _levelService.GetLevels(
                new Services.Requests.Level.GetLevelsRequest {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending} }
                }).Levels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString()}).ToList();
            viewModel.IsActive = true;

            return viewModel;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateRoleGroupViewModel();
            viewModel = CreateViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateRoleGroupViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateRoleGroupRequest>();
            request.ControllerName = "RoleGroup";
            request.ActionName = "Create";
            request.UserId = this.UserProfile().UserId;
            var response = _roleGroupService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public UpdateRoleGroupViewModel UpdateViewModel(UpdateRoleGroupViewModel viewModel)
        {
            viewModel.LevelList = _levelService.GetLevels(
                new Services.Requests.Level.GetLevelsRequest
                {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                }).Levels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            return viewModel;
        }

        public ActionResult Update(int id)
        {
            var response = _roleGroupService.GetRoleGroup(new GetRoleGroupRequest { Id = id });
            var viewModel = response.MapTo<UpdateRoleGroupViewModel>();
            viewModel = UpdateViewModel(viewModel);

            return View(viewModel);
        }

        
        [HttpPost]
        public ActionResult Update(UpdateRoleGroupViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateRoleGroupRequest>();
            var response = _roleGroupService.Update(request);
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
            var response = _roleGroupService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var role = _roleGroupService.GetRoleGroups(new GetRoleGroupsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = role.TotalRecords,
                iTotalRecords = role.RoleGroups.Count,
                aaData = role.RoleGroups
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}