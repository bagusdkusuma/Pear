using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DSLNG.PEAR.Web.ViewModels.Kpi
{
    public class UploadViewModel
    {
        [DataType(DataType.Upload)]
        public HttpPostedFileBase IconFile { get; set; }
        public string ReturnUrl
        {
            get
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                string action = HttpContext.Current.Request.RequestContext.RouteData.Values["Action"].ToString();
                string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"].ToString();
                if (action == "Update")
                {
                    int id = Int32.Parse(HttpContext.Current.Request.RequestContext.RouteData.Values["Id"].ToString());
                    return urlHelper.Action(action, controller, new { Id = id });
                }
                return urlHelper.Action(action, controller);
            }
        }
    }
}