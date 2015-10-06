using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Menu;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Web.DependencyResolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public const string UploadDirectory = "~/Content/UploadedFiles/";
        public const string TemplateDirectory = "~/Content/TemplateFiles/";
        public ContentResult ErrorPage(string message)
        {
            return Content(message);
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
            base.OnActionExecuted(filterContext);
            filterContext.Controller.ViewBag.BodyClass = "";
            var absolutePath = filterContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
            if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest()) {
                var _menuService = ObjectFactory.Container.GetInstance<IMenuService>();
                var rootMenu = _menuService.GetRootMenu(new GetRootMenuRequest { AbsolutePath = absolutePath });
                if (!string.IsNullOrEmpty(rootMenu.RootName)) {
                    filterContext.Controller.ViewBag.BodyClass = rootMenu.RootName.ToLower();
                }
            }
        }
    }
}
