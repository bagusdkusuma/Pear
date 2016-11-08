using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.User;
using DSLNG.PEAR.Common.Extensions;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Web.Attributes;
using System.Data.SqlClient;
using System.IO;
using DSLNG.PEAR.Web.ViewModels.RolePrivilege;
using System;
using DSLNG.PEAR.Services.Requests.Privilege;

namespace DSLNG.PEAR.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleGroupService _roleGroupService;
        private readonly IRolePrivilegeService _rolePrivilegeService;

        public UserController(IUserService userService, IRoleGroupService roleGroupService, IRolePrivilegeService rolePrivilegeService)
        {
            _userService = userService;
            _roleGroupService = roleGroupService;
            _rolePrivilegeService = rolePrivilegeService;
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

            if (response.IsSuccess)
            {
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
            viewModel.RoleGroupList = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                .RoleGroups.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = viewModel.RoleId == x.Id ? true : false
                }).ToList();
            viewModel.IsActive = true;
            viewModel.RolePrivilegeOption = _rolePrivilegeService.GetRolePrivileges(new Services.Requests.Privilege.GetPrivilegeByRoleRequest { RoleId = viewModel.RoleId }).Privileges.Select(x => new SelectListItem
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
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/signature/"))))
                    {
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/signature/")));
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/signature/"), fileName);
                    var url = "/Content/signature/" + fileName;

                    file.SaveAs(path);
                    viewModel.SignatureImage = url;
                }
            }
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

            viewModel.RoleGroupList = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                .RoleGroups.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = viewModel.RoleId == x.Id ? true : false
                }).ToList();
            viewModel.RolePrivilegeOption = _rolePrivilegeService.GetRolePrivileges(new Services.Requests.Privilege.GetPrivilegeByRoleRequest { RoleId = viewModel.RoleId }).Privileges.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return viewModel;
        }

        public ActionResult Update(int id)
        {
            var response = _userService.GetUser(new GetUserRequest { Id = id });
            var viewModel = response.MapTo<UpdateUserViewModel>();
            viewModel = UpdateViewModel(viewModel);
            // viewModel.RolePrivilegeOption = _roleGroupService.GetRoleGroup(new Services.Requests.RoleGroup.GetRoleGroupRequest { Id = viewModel.RoleId }).Privileges.MapTo<SelectListItem>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetPrivilege(int roleId)
        {
            List<SelectListItem> data = new List<SelectListItem>();
            if (roleId > 0)
            {
                var role = _roleGroupService.GetRoleGroup(new Services.Requests.RoleGroup.GetRoleGroupRequest { Id = roleId });
                if (role.IsSuccess && role.Privileges != null)
                {
                    foreach (var privilege in role.Privileges)
                    {
                        data.Add(new SelectListItem
                        {
                            Text = privilege.Name,
                            Value = privilege.Id.ToString()
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(UpdateUserViewModel viewModel)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/signature/"))))
                    {
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/signature/")));
                    }
                    var path = Path.Combine(Server.MapPath("~/Content/signature/"), fileName);
                    var url = "/Content/signature/" + fileName;

                    file.SaveAs(path);
                    viewModel.SignatureImage = url;
                }
            }
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

        public ActionResult Grid(GridParams gridParams)
        {
            var users = _userService.GetUsers(new GetUsersRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = users.Users.Count,
                iTotalDisplayRecords = users.TotalRecords,
                aaData = users.Users
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddPrivilege(int RoleId)
        {
            var model = new RolePrivilegeViewModel();
            ViewBag.RoleGroups = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                            .RoleGroups.Select(x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString(),
                                Selected = RoleId == x.Id
                            }).ToList();
            model.RoleGroup_Id = RoleId;
            var roles = _rolePrivilegeService.GetMenuRolePrivileges(new GetPrivilegeByRolePrivilegeRequest { RoleId = RoleId });
            if (roles.IsSuccess)
            {
                model.MenuRolePrivileges = roles.MenuRolePrivileges.ToList().MapTo<RolePrivilegeViewModel.MenuRolePrivilege>();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPrivilege(RolePrivilegeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = model.MapTo<SaveRolePrivilegeRequest>();
                request.UserId = this.UserProfile().UserId;
                var result = _rolePrivilegeService.SaveRolePrivilege(request);
                return Json(result);
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { IsSuccess = false, Message = errorList });
            }
            //ViewBag.RoleGroups = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            //{
            //    Take = -1,
            //    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            //})
            //                .RoleGroups.Select(x => new SelectListItem
            //                {
            //                    Text = x.Name,
            //                    Value = x.Id.ToString(),
            //                    Selected = model.RoleGroup_Id == x.Id
            //                }).ToList();
            //return View(model);
        }
        private List<SelectListItem> GetRoleGroupOptionList(int? roleId)
        {
            List<SelectListItem> roles = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                .RoleGroups.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = roleId == x.Id ? true : false
                }).ToList();

            return roles;
        }
    }
}