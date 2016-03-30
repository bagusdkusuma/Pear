using DSLNG.PEAR.Web.ViewModels.MirDataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.MirDataTable;
using DSLNG.PEAR.Services.Interfaces;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MirDataTableController : BaseController
    {
        private readonly IMirDataTableService _mirDataTableService;
        public MirDataTableController(IMirDataTableService mirDataTableService)
        {
            _mirDataTableService = mirDataTableService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(SaveMirDataTableViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveMirDataTableRequest>();
            var response = _mirDataTableService.SaveMirDataTableRespons(request);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult Delete(DeleteMirDataTableViewModel viewModel)
        //{

        //}


	}
}