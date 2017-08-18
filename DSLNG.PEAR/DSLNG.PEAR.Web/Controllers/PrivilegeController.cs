using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Privilege;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.RolePrivilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using System.Data.SqlClient;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PrivilegeController : BaseController
    {
        // GET: Privilege
        private readonly IRolePrivilegeService _roleService;
        private readonly IRoleGroupService _roleGroupService;
        public PrivilegeController(IRolePrivilegeService roleService, IRoleGroupService roleGroupService)
        {
            _roleService = roleService;
            _roleGroupService = roleGroupService;
        }
        public ActionResult Index(int? roleId)
        {
            var RoleId = roleId.HasValue ? roleId.Value : this.UserProfile().RoleId;
            List<RolePrivilegeViewModel> model = new List<RolePrivilegeViewModel>();
            var data = _roleService.GetRolePrivileges(new GetPrivilegeByRoleRequest { RoleId = RoleId });
            if (data.IsSuccess && data.Privileges.Count > 0)
            {
                model = data.Privileges.MapTo<RolePrivilegeViewModel>();
            }

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

            ViewBag.RoleId = RoleId;
            return View(model);
        }

        public ActionResult Grid(GridParams gridParams, int? roleId)
        {
            roleId = roleId.HasValue ? roleId.Value : this.UserProfile().RoleId;
            var rpv = _roleService.GetRolePrivileges(new GetPrivilegesRequest
            {
                RoleId = roleId.Value,
                Take = gridParams.DisplayLength,
                Skip = gridParams.DisplayStart,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });
            List<RolePrivilegeViewModel> DataResponse = rpv.Privileges.MapTo<RolePrivilegeViewModel>();
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = rpv.TotalRecords,
                iTotalRecords = rpv.Privileges.Count,
                aaData = DataResponse
            };
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult Edit(int Id)
        {
            int RoleId = this.UserProfile().RoleId;
            var result = _roleService.GetRolePrivilege(new GetPrivilegeRequest
            {
                Id = Id
            });
            var model = new RolePrivilegeViewModel();
            model = result.MapTo<RolePrivilegeViewModel>();
            
            if (model != null)
            {
                RoleId = model.RoleGroup_Id > 0 ? model.RoleGroup_Id : this.UserProfile().RoleId;
            }

            var privilege = _roleService.GetMenuRolePrivileges(new GetPrivilegeByRolePrivilegeRequest
            {
                RoleId = RoleId,
                RolePrivilegeId = Id
            });

            model.MenuRolePrivileges = privilege.MenuRolePrivileges.ToList().MapTo<RolePrivilegeViewModel.MenuRolePrivilege>();

            ViewBag.RoleGroups = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                            .RoleGroups.Select(x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString(),
                                Selected = RoleId  == x.Id
                            }).ToList();


            return View(model);
        }

        public ActionResult Create(int RoleId)
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
            var roles = _roleService.GetMenuRolePrivileges(new GetPrivilegeByRolePrivilegeRequest { RoleId = RoleId });
            if (roles.IsSuccess)
            {
                model.MenuRolePrivileges = roles.MenuRolePrivileges.ToList().MapTo<RolePrivilegeViewModel.MenuRolePrivilege>();
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RolePrivilegeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = model.MapTo<SaveRolePrivilegeRequest>();
                request.UserId = this.UserProfile().UserId;
                request.ControllerName = "Privilege";
                request.ActionName = "Create";
                if (_roleService.SaveRolePrivilege(request).IsSuccess)
                {
                    return RedirectToAction("Index", new { roleId = request.RoleGroup_Id });
                }

            }
            ViewBag.RoleGroups = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                            .RoleGroups.Select(x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString(),
                                Selected = model.RoleGroup_Id == x.Id
                            }).ToList();

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(RolePrivilegeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = model.MapTo<SaveRolePrivilegeRequest>();
                request.UserId = this.UserProfile().UserId;
                request.ControllerName = "Privilege";
                request.ActionName = "Edit";
                var response = _roleService.SaveRolePrivilege(request);
                
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index", new { roleId = request.RoleGroup_Id });
                }

            }
            ViewBag.RoleGroups = _roleGroupService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            })
                            .RoleGroups.Select(x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString(),
                                Selected = model.RoleGroup_Id == x.Id
                            }).ToList();

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var response = new BaseResponse();
            response = _roleService.DeleteRolePrivilege(new DeleteRolePrivilegeRequest
            {
                Id = id,
                UserId = this.UserProfile().UserId,
                ControllerName = "Role Privilege",
                ActionName = "Delete"
            });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }
    }
}