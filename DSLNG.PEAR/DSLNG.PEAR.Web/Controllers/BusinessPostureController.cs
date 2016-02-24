using DSLNG.PEAR.Web.ViewModels.BusinessPosture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.BusinessPosture;
using DSLNG.PEAR.Services.Interfaces;

namespace DSLNG.PEAR.Web.Controllers
{
    public class BusinessPostureController : BaseController
    {
        private readonly IBusinessPostureIdentificationService _businessPostureService;
        public BusinessPostureController(IBusinessPostureIdentificationService businessPostureService) {
            _businessPostureService = businessPostureService;
        }
        [HttpPost]
        public ActionResult AddDesiredState(DesiredStateViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveDesiredStateRequest>();
            return Json(_businessPostureService.SaveDesiredState(request));
        }
        [HttpPost]
        public ActionResult DeleteDesiredState(int id) {
            var request = new DeleteDesiredStateRequest { Id = id };
            return Json(_businessPostureService.DeleteDesiredState(request));
        }
        [HttpPost]
        public ActionResult AddPostureChallenge(PostureChallengeViewModel viewModel) {
            var request = viewModel.MapTo<SavePostureChallengeRequest>();
            var response = _businessPostureService.SavePostureChallenge(request);
            var data = new
            {
                Id = response.Id,
                RelationIds = response.RelationIds,
                Definition = response.Definition,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                PostureType = viewModel.PostureType
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePostureChallenge(int id)
        {
            var request = new DeletePostureChallengeRequest { Id = id };
            return Json(_businessPostureService.DeletePostureChallenge(request));
        }
        [HttpPost]
        public ActionResult AddPostureConstraint(PostureConstraintViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePostureConstraintRequest>();
            var response = _businessPostureService.SavePostureConstraint(request);
            var data = new
            {
                Id = response.Id,
                RelationIds = response.RelationIds,
                Definition = response.Definition,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                PostureType = viewModel.PostureType
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePostureConstraint(int id)
        {
            var request = new DeletePostureConstraintRequest { Id = id };
            return Json(_businessPostureService.DeletePostureConstraint(request));
        }

        [HttpPost]
        public PartialViewResult GetPostureChallenge(int id)
        {
            var viewModel = _businessPostureService.GetPostureChallenge(new GetPostureChallengeRequest { Id = id }).MapTo<PostureChalengeListViewModel>();

            return PartialView("_showPostureChallenge", viewModel);
        }

        [HttpPost]
        public PartialViewResult GetPostureConstraint(int id)
        {
            var viewModel = _businessPostureService.GetPostureConstraint(new GetPostureConstraintRequest { Id = id }).MapTo<PostureConstraintListViewModel>();
            return PartialView("_showPostureConstraint", viewModel);
        }
	}
}