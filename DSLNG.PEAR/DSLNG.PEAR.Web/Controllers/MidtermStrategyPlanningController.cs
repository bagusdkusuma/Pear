using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.ViewModels.MidtermStrategyPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.MidtermPlanning;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MidtermStrategyPlanningController : BaseController
    {
        private readonly IMidtermPlanningService _midtermPlanningService;

        public MidtermStrategyPlanningController(IMidtermPlanningService midtermPlanningService) {
            _midtermPlanningService = midtermPlanningService;
        }
        //
        // GET: /MidtermStrategyPlanning/GetByStageId/:id
        public ActionResult GetByStageId(int id)
        {
            return Json(_midtermPlanningService.GetByStageId(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddObjective(MidtermPlanningObjectiveViewModel viewModel) {
            return Json(_midtermPlanningService.AddObejctive(viewModel.MapTo<AddObjectiveRequest>()));
        }

        [HttpPost]
        public ActionResult DeleteObjective(int id) {
            return Json(_midtermPlanningService.DeleteObjective(id));
        }

        [HttpPost]
        public ActionResult AddKpi(AddPlanningKpiViewModel viewModel) {
            return Json(_midtermPlanningService.AddKpi(viewModel.MapTo<AddPlanningKpiRequest>()));
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            return Json(_midtermPlanningService.Delete(id));
        }
        
        [HttpPost]
        public ActionResult DeleteKpi(int id, int midTermId)
        {
            return Json(_midtermPlanningService.DeleteKpi(id, midTermId));
        }

        [HttpPost]
        public ActionResult Add(AddMidtermPlanningViewModel viewModel)
        {
            return Json(_midtermPlanningService.Add(viewModel.MapTo<AddMidtermPlanningRequest>()));
        }

        public ActionResult IsValid(int id) {
            return Json(new { IsValid = _midtermPlanningService.IsValid(id) }, JsonRequestBehavior.AllowGet);
        }
	}
}