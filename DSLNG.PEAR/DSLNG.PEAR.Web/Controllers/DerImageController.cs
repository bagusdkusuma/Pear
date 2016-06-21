using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.Der;
using DSLNG.PEAR.Web.DependencyResolution;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerImageController : Controller
    {
        private readonly IDerService _derService;
        public static string SecretNumber { get; set; }
        public DerImageController(IDerService derService) {
            _derService = derService;
        }

        public ActionResult LayoutItem(int id, string currentDate)
        {
            var derController = ObjectFactory.Container.GetInstance<DerController>();
            derController.ControllerContext = new ControllerContext(Request.RequestContext, derController);
            return derController.LayoutItem(id, currentDate);
        }

        public ActionResult Preview(string secretNumber)
        {
            //if (DerImageController.SecretNumber != secretNumber) {
            //    return HttpNotFound();
            //}
            var activeDer = _derService.GetActiveDer();
            var id = activeDer.Id;
            var response = _derService.GetDerLayout(id);
            var viewModel = response.MapTo<DerDisplayViewModel>();
            return View("Preview2",viewModel);
            //return View("~/Views/Der/Preview2.cshtml",viewModel);
        }
	}
}