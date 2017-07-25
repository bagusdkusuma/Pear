using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        //private readonly IMenuService _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
        public string AccessLevel { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
            var sessionData = (UserProfileSessionData)httpContext.Session["LoginUser"];
            //by passing super admin
            if (sessionData == null) return false;
            var controller = httpContext.Request.RequestContext.RouteData.Values["Controller"].ToString();

            if (sessionData.IsSuperAdmin)
            {
                GetAllRights(controller);
                return true;
            }

            var action = httpContext.Request.RequestContext.RouteData.Values["Action"].ToString();
            var url = httpContext.Request.RawUrl;
            var urlHelperUrl = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string cleanUrl = urlHelperUrl.Action(action, controller, new { id = string.Empty });
            var modul = string.Format("/{0}/", controller);
            var menu = _menuService.GetMenuByUrl(new DSLNG.PEAR.Services.Requests.Menu.GetMenuRequestByUrl { Url = modul, RoleId = sessionData.RoleId });
            if (menu == null || menu.IsSuccess == false) return false;

            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;
            string privilegeLevels = string.Join(",", GetUserRights(menu.Id, controller, sessionData.RolePrivilegeName));

            var userRight = httpContext.Session[controller];
            if (userRight == null)
            {
                httpContext.Session[controller] = privilegeLevels;
            }

            if (privilegeLevels.Contains(this.AccessLevel)) return true;
            else return false;
        }

        private void GetAllRights(string controller)
        {
            List<string> userRights = (List<string>)HttpContext.Current.Session[controller] != null ? (List<string>)HttpContext.Current.Session[controller] : new List<string> { 
                "AllowApprove","AllowView","AllowCreate","AllowDelete","AllowDownload","AllowUpload","AllowPublish","AllowUpdate","AllowInput"
            };
            HttpContext.Current.Session[controller] = userRights;
        }

        private IEnumerable<string> GetUserRights(int menu_id, string menu_name, List<KeyValuePair<int, string>> currentPrivileges)
        {
            var _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
            List<string> userRights = (List<string>)HttpContext.Current.Session[menu_name] != null ? (List<string>)HttpContext.Current.Session[menu_name] : new List<string>();
            foreach (var item in currentPrivileges)
            {
                var privilege = _menuService.GetMenuPrivilege(new DSLNG.PEAR.Services.Requests.Menu.GetMenuPrivilegeRequest { Menu_Id = menu_id, RolePrivilege_Id = item.Key });
                if (!privilege.IsSuccess) continue;
                if (privilege.AllowApprove)
                {
                    if (!userRights.Contains("AllowApprove"))
                        userRights.Add("AllowApprove");
                }
                if (privilege.AllowView)
                {
                    if (!userRights.Contains("AllowView"))
                        userRights.Add("AllowView");
                }
                if (privilege.AllowCreate)
                {
                    if (!userRights.Contains("AllowCreate"))
                        userRights.Add("AllowCreate");
                }
                if (privilege.AllowDelete)
                {
                    if (!userRights.Contains("AllowDelete"))
                        userRights.Add("AllowDelete");
                }
                if (privilege.AllowDownload)
                {
                    if (!userRights.Contains("AllowDownload"))
                        userRights.Add("AllowDownload");
                }
                if (privilege.AllowPublish)
                {
                    if (!userRights.Contains("AllowPublish"))
                        userRights.Add("AllowPublish");
                }
                if (privilege.AllowUpdate)
                {
                    if (!userRights.Contains("AllowUpdate"))
                        userRights.Add("AllowUpdate");
                }
                if (privilege.AllowUpload)
                {
                    if (!userRights.Contains("AllowUpload"))
                        userRights.Add("AllowUpload");
                }
                if (privilege.AllowInput)
                {
                    if (!userRights.Contains("AllowInput"))
                        userRights.Add("AllowInput");
                }
            }
            HttpContext.Current.Session[menu_name] = userRights;
            return userRights.AsEnumerable<string>();
        }



        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
            {
                { "controller", "UnAuthorized" }, 
                { "action", "Error" } 
            });
        }
    }
}