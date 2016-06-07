using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Services.Requests.Menu;

namespace DSLNG.PEAR.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class PearMenuAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["Controller"].ToString();
            var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["Action"].ToString();
            var url = filterContext.HttpContext.Request.RawUrl;
            var menuService = ObjectFactory.Container.GetInstance<IMenuService>();
            var urlHelperUrl = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string cleanUrl = urlHelperUrl.Action(action, controller, new {id = string.Empty});
            var rootMenuActive = menuService.GetSiteMenuActive(new GetSiteMenuActiveRequest() { Action = action, Controller = controller, Url = url, CleanUrl = cleanUrl });
            if(filterContext.Controller.TempData["RootMenuActive"] != null)
            {
                filterContext.Controller.TempData["RootMenuActive"] = rootMenuActive;
            }
            else
            {
                filterContext.Controller.TempData.Add("RootMenuActive", rootMenuActive);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}