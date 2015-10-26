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

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public const string UploadDirectory = "~/Content/UploadedFiles/";
        public const string TemplateDirectory = "~/Content/TemplateFiles/";
        private UserProfileSessionData _userinfo;
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
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Session["LoginUser"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                { 
                    { "controller", "Account" }, 
                    { "action", "Login" } 
                });
            }
            else
            {
                if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    var sessionData = (UserProfileSessionData)this.Session["LoginUser"];
                    if (!sessionData.IsSuperAdmin)
                    {
                        var currentUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
                        if (currentUrl.Length > 1)
                        {
                            if (currentUrl != "/UnAuthorized/Error")
                            {
                                var menuService = ObjectFactory.Container.GetInstance<IMenuService>();
                                var menu = menuService.GetMenuByUrl(new GetMenuRequestByUrl { Url = currentUrl, RoleId = sessionData.RoleId });
                                if (menu == null || menu.IsSuccess == false)
                                {
                                    filterContext.Result = new RedirectToRouteResult(
                                            new RouteValueDictionary 
                                { 
                                    { "controller", "UnAuthorized" }, 
                                    { "action", "Error" } 
                                });
                                }
                            }
                        }
                    }
                }
            }
            base.OnAuthorization(filterContext);
        }
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
    }
}
