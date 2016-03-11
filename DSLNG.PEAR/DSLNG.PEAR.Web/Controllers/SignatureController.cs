using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.ViewModels.PopDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Signature;

namespace DSLNG.PEAR.Web.Controllers
{
    public class SignatureController : BaseController
    {
        private readonly ISignatureService _signatureService;
        private readonly IDropdownService _dropdownService;
        public SignatureController(ISignatureService signatureService, IDropdownService dropdownService)
        {
            _signatureService = signatureService;
            _dropdownService = dropdownService;
        }

        [HttpPost]
        public ActionResult Create(GetPopDashboardViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveSignatureRequest>();
            var response = _signatureService.SaveSignature(request);
            var data = new
            {
                id = response.Id,
                userId = response.UserId,
                username = response.Username,
                type = response.TypeSignature.ToString(),
                typeInt = response.TypeSignature,
                signatureImage = response.SignatureImage
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(GetPopDashboardViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveSignatureRequest>();
            var response = _signatureService.SaveSignature(request);
            var data = new
            {
                id = response.Id,
                userId = response.UserId,
                username = response.Username,
                type = response.TypeSignature.ToString(),
                typeInt = response.TypeSignature,
                signatureImage = response.SignatureImage
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Approve(SaveApprovalViewModel viewModel)
        {
            var request = viewModel.MapTo<ApproveSignatureRequest>();
            var response = _signatureService.ApproveSignature(request);
            var data = new
            {
                id = response.Id,
                userId = response.UserId,
                type = response.Type.ToString(),
                signatureImage = response.SignatureImage,
                approve = response.Approve,
                reject = response.Reject,
                note = response.Note
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}