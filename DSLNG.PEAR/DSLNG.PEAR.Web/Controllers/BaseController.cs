using System.IO;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Menu;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;
using DSLNG.PEAR.Web.ViewModels.Menu;
using DSLNG.PEAR.Web.ViewModels.User;

namespace DSLNG.PEAR.Web.Controllers
{
    //[Authorize]
    public class BaseController : Controller
    {
        public const string UploadDirectory = "~/Content/UploadedFiles/";
        public const string TemplateDirectory = "~/Content/TemplateFiles/";
        //private UserProfileSessionData _userinfo;
        private readonly IMenuService _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
        private readonly IAuditTrailService _auditService = ObjectFactory.Container.GetInstance<IAuditTrailService>();
        public virtual new CustomPrincipal User
        {
            get
            {
                return HttpContext.User as CustomPrincipal;
            }
        }
        public ContentResult ErrorPage(string message)
        {
            return Content(message);
        }

        public UserProfileSessionData UserProfile()
        {
            return (UserProfileSessionData)this.Session["LoginUser"];
        }
        //protected override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    bool Authorized = false;
        //    //if(Request.IsAuthenticated){
        //    //    var testId = WebSecurity.CurrentUserId;
        //    //}
        //    //var userService = ObjectFactory.Container.GetInstance<IUserService>();
        //    //var userRole = userService.GetUser(new Services.Requests.User.GetUserRequest { Id = 1, Email = "" });
        //    //var roles = userRole.Role;

        //    if (Request.IsAuthenticated)
        //    {
        //        var userService = ObjectFactory.Container.GetInstance<IUserService>();
        //        var AuthUser = userService.GetUserByName(new GetUserByNameRequest { Name = HttpContext.User.Identity.Name });
        //        if (AuthUser.IsSuccess == true)
        //        {
        //            var role = AuthUser.Role;
        //            var currentUrl = filterContext.HttpContext.Request.RawUrl;
        //            if (currentUrl.Length > 1)
        //            {
        //                var menuService = ObjectFactory.Container.GetInstance<IMenuService>();
        //                var menu = menuService.GetMenuByUrl(new GetMenuRequestByUrl { Url = currentUrl, RoleId = role.Id });
        //                if (menu == null || menu.IsSuccess == false)
        //                {
        //                    throw new UnauthorizedAccessException("You don't have authorization to view this page, please contact system administrator if you have authorization to this page");
        //                    //RedirectToAction("Error", "UnAuthorized");
        //                }
        //            }
        //        }

        //    }
        //    else {
        //        throw new UnauthorizedAccessException("You don't have authorization to view this page, please contact system administrator if you have authorization to this page");
        //    }
        //    //else
        //    //{
        //    //    //throw new UnauthorizedAccessException("You don't have authorization to view this page, please contact system administrator if you have authorization to this page");
        //    //    //RedirectToAction("Login", "Account");

        //    //}
        //    ////var menuService = ObjectFactory.Container.GetInstance<IMenuService>();
        //    ////var menu = menuService.GetMenuByRole(new Services.Requests.Menu.GetMenuRequestByRoleId { RoleId = roles.Id });
        //    ////bool authorized = true;
        //    ////jika gagal login
        //    ////throw new UnauthorizedAccessException("message");

        //    base.OnAuthorization(filterContext);
        //}
        //protected override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (Session["LoginUser"] == null)
        //    {
        //        filterContext.Result = new RedirectToRouteResult(
        //        new RouteValueDictionary 
        //        { 
        //            { "controller", "Account" }, 
        //            { "action", "Login" } 
        //        });
        //    }
        //    #region authorize
        //    //else
        //    //{
        //    //    if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
        //    //    {
        //    //        var sessionData = (UserProfileSessionData)this.Session["LoginUser"];
        //    //        if (!sessionData.IsSuperAdmin)
        //    //        {

        //    //            var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["Controller"].ToString();
        //    //            var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["Action"].ToString();
        //    //            var urlHelperUrl = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        //    //            string cleanUrl = urlHelperUrl.Action(action, controller, new { id = string.Empty });
        //    //            //var currentUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
        //    //            if (cleanUrl.Length > 1)
        //    //            {
        //    //                if (cleanUrl != "/UnAuthorized/Error")
        //    //                {
        //    //                    var menuService = ObjectFactory.Container.GetInstance<IMenuService>();
        //    //                    var menu = menuService.GetMenuByUrl(new GetMenuRequestByUrl { Url = cleanUrl, RoleId = sessionData.RoleId });
        //    //                    if (menu == null || menu.IsSuccess == false)
        //    //                    {
        //    //                        filterContext.Result = new RedirectToRouteResult(
        //    //                                new RouteValueDictionary 
        //    //                    { 
        //    //                        { "controller", "UnAuthorized" }, 
        //    //                        { "action", "Error" } 
        //    //                    });
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        GetMenuPrivilege(menu.Id, sessionData.RolePrivilegeName);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    #endregion
        //    base.OnAuthorization(filterContext);
        //}

        //private void GetMenuPrivilege(int menu_id, List<KeyValuePair<int, string>> privileges)
        //{
        //    MenuPrivilegeViewModel Global = new MenuPrivilegeViewModel();
        //    foreach (var item in privileges)
        //    {
        //        var privilege = _menuService.GetMenuPrivilege(new GetMenuPrivilegeRequest { Menu_Id = menu_id, RolePrivilege_Id = item.Key });
        //        if (!privilege.IsSuccess) continue;
        //        Global.Menu_Id = menu_id;
        //        Global.Menu_Name = privilege.Menu.Name;
        //        Global.Menu_Url = privilege.Menu.Url;
        //        Global.AllowApprove = Global.AllowApprove == false ? privilege.AllowApprove : Global.AllowApprove;
        //        Global.AllowCreate = Global.AllowCreate == false ? privilege.AllowCreate : Global.AllowCreate;
        //        Global.AllowDelete = Global.AllowDelete == false ? privilege.AllowDelete : Global.AllowDelete;
        //        Global.AllowDownload = Global.AllowDownload == false ? privilege.AllowDownload : Global.AllowDownload;
        //        Global.AllowPublish = Global.AllowPublish == false ? privilege.AllowPublish : Global.AllowPublish;
        //        Global.AllowUpdate = Global.AllowUpdate == false ? privilege.AllowUpdate : Global.AllowUpdate;
        //        Global.AllowUpload = Global.AllowUpload == false ? privilege.AllowUpload : Global.AllowUpload;
        //        Global.AllowView = Global.AllowView == false ? privilege.AllowView : Global.AllowView;
        //        //GetMenuPrivilege(menu_id, item.Key);
        //    }
        //    Session[Global.Menu_Url.ToString()] = Global;
        //    //return authorized;
        //}
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(UnauthorizedAccessException))
            {
                //Redirect user to error page
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Login", "Account", new { message = filterContext.Exception.Message });
            }
            base.OnException(filterContext);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session.IsNewSession || Session["LoginUser"] == null)
            {
                //filterContext.Result = Json("Session Timeout", "text/html", JsonRequestBehavior.AllowGet);
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                });
            }

            if (Session["LoginUser"] != null && !filterContext.RequestContext.RouteData.Values["Action"].ToString().Equals("SiteMap")
                && !filterContext.RequestContext.RouteData.Values["Action"].ToString().Contains("Grid") && !filterContext.RequestContext.RouteData.Values["Controller"].ToString().Contains("Audit") && !filterContext.RequestContext.RouteData.Values["Controller"].ToString().Contains("Home"))
            {
                var request = new Services.Requests.AuditTrail.CreateAuditUserRequest();
                request.UserId = this.UserProfile().UserId;
                request.Login_Id = this.UserProfile().LoginId;
                request.ControllerName = filterContext.RequestContext.RouteData.Values["Controller"].ToString();
                request.ActionName = filterContext.RequestContext.RouteData.Values["Action"].ToString();
                request.Url = filterContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
                _auditService.CreateAuditUserRequest(request);
            }
            base.OnActionExecuted(filterContext);
            filterContext.Controller.ViewBag.BodyClass = "";
            var absolutePath = filterContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
            if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                var _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
                var rootMenu = _menuService.GetRootMenu(new GetRootMenuRequest { AbsolutePath = absolutePath });
                if (!string.IsNullOrEmpty(rootMenu.RootName))
                {
                    filterContext.Controller.ViewBag.BodyClass = rootMenu.RootName.ToLower();
                }
            }
        }

        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
