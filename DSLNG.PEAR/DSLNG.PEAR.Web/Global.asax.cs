//using DSLNG.PEAR.Data.Installer;

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Web.App_Start;
using DSLNG.PEAR.Web.AutoMapper;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Web.Scheduler;
using FluentScheduler;
using StructureMap;
using System;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DSLNG.PEAR.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiTestClient.WebApiTestClientHttpMessageHandler.RegisterRouteForTestClient(GlobalConfiguration.Configuration);
            AutoMapperConfiguration.Configure();
            //Database.SetInitializer<DataContext>(new DataInitializer());
            Database.SetInitializer<DataContext>(null);

            //StructureMap Container
            IContainer container = IoC.Initialize();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapDependencyResolver(container);
            
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            JobManager.JobFactory = new StructureMapJobFactory();
            JobManager.Initialize(new ScenarioScheduler());
            //JobManager.Initialize(new Registry());

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }
        //protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        //{
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

        //    if (authCookie != null)
        //    {
        //        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

        //        if (authTicket.UserData != null)
        //        {
        //            JavaScriptSerializer serializer = new JavaScriptSerializer();
        //            UserViewModel serializeModel = serializer.Deserialize<UserViewModel>(authTicket.UserData);
        //            CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
        //            newUser.Id = serializeModel.Id;
        //            newUser.Username = serializeModel.Username;
        //            newUser.RoleName = serializeModel.RoleName;
        //            newUser.IsSuperAdmin = serializeModel.IsSuperAdmin;
        //            newUser.Email = serializeModel.Email;
        //            HttpContext.Current.User = newUser;
        //        }
                

        //    }
        //}
    }
}