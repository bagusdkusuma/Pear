using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Web.ViewModels.User;
using DSLNG.PEAR.Common.Extensions;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Web.Attributes;

namespace DSLNG.PEAR.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleGroupService _roleGroupService;

        public UserController(IUserService userService, IRoleGroupService roleGroupService)
        {
            _userService = userService;
            _roleGroupService = roleGroupService;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLoginViewModel viewModel)
        {
            var request = viewModel.MapTo<LoginUserRequest>();
            var response = _userService.Login(request);

            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;

            if (response.IsSuccess){
                //save user id and rolegroup to session

                return RedirectToAction("Index");
            }

            return View("Login", viewModel);
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridUserIndex");
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
            viewModel.Columns.Add("Username");
            viewModel.Columns.Add("Email");
            viewModel.Columns.Add("IsActive");
            viewModel.Columns.Add("RoleName");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridUserIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _userService.GetUsers(new GetUsersRequest()).Users.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _userService.GetUsers(new GetUsersRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Users;
        }

        public CreateUserViewModel CreateViewModel(CreateUserViewModel viewModel)
        {
            viewModel.RoleGroupList = _roleGroupService.GetRoleGroups(
                new Services.Requests.RoleGroup.GetRoleGroupsRequest { Skip = 0, Take = 0 }).RoleGroups.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            return viewModel;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateUserViewModel();
            viewModel = CreateViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateUserViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateUserRequest>();
            var response = _userService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public UpdateUserViewModel UpdateViewModel(UpdateUserViewModel viewModel)
        {
            viewModel.RoleGroupList = _roleGroupService.GetRoleGroups(
                new Services.Requests.RoleGroup.GetRoleGroupsRequest { Skip = 0, Take = 0 }).RoleGroups.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = viewModel.RoleId==x.Id ? true : false
                }).ToList();

            return viewModel;
        }

        public ActionResult Update(int id)
        {
            var response = _userService.GetUser(new GetUserRequest { Id = id });
            var viewModel = response.MapTo<UpdateUserViewModel>();

            viewModel = UpdateViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateUserViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateUserRequest>();
            var response = _userService.Update(request);
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
            var response = _userService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        
	}
}