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
        public ActionResult Index()
        {
            List<RolePrivilegeViewModel> model = new List<RolePrivilegeViewModel>();
            var data = _roleService.GetRolePrivileges(new GetPrivilegeByRoleRequest { RoleId = this.UserProfile().RoleId });
            if(data.IsSuccess && data.Privileges.Count > 0)
            {
                model = data.Privileges.MapTo<RolePrivilegeViewModel>();
            }
            var roles = _roleGroupService.All().RoleGroups.ToDictionary(x => x.Id, y=>y.Name);
            ViewBag.RoleGroups = roles.MapTo<SelectListItem>();
            return View(model);
        }

        public ActionResult Grid(GridParams gridParams, int? roleId)
        {
            roleId = roleId.HasValue ? roleId.Value : this.UserProfile().RoleId;
            var rpv = _roleService.GetRolePrivileges(new GetPrivilegesRequest {
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
    }
}