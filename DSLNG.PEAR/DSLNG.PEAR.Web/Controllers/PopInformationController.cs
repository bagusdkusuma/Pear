using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.ViewModels.PopDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.PopInformation;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PopInformationController : BaseController
    {
        private readonly IPopInformationService _popInformationService;
        public PopInformationController(IPopInformationService popInformationService)
        {
            _popInformationService = popInformationService;
        }

        [HttpPost]
        public ActionResult Create(SavePopInformationViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePopInformationRequest>();
            var response = _popInformationService.SavePopInformation(request);
            var data = new
            {
                id = response.Id,
                type = response.Type.ToString(),
                typeInt = response.Type,
                title = response.Title,
                value = response.Value
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(SavePopInformationViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePopInformationRequest>();
            var response = _popInformationService.SavePopInformation(request);
            var data = new
            {
                id = response.Id,
                type = response.Type.ToString(),
                typeInt = response.Type,
                title = response.Title,
                value = response.Value
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

	}
}