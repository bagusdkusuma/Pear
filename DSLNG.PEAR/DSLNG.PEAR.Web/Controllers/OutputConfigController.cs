using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.OutputCategory;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Web.ViewModels.OutputConfig;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class OutputConfigController : BaseController
    {
        private readonly IMeasurementService _measurementService;
        private readonly IOutputCategoryService _outputCategoryService;
        private readonly IOutputConfigService _outputConfigService;

        public OutputConfigController(IMeasurementService measurementService,IOutputCategoryService outputCategoryService,
            IOutputConfigService outputConfigService) {
            _measurementService = measurementService;
            _outputCategoryService = outputCategoryService;
            _outputConfigService = outputConfigService;
        }
        //
        // GET: /OutputConfig/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create() {
            var viewModel = new OutputConfigViewModel();
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest { 
                Take = -1,
                SortingDictionary = new SortedDictionary<string,SortOrder>{{"Name",SortOrder.Ascending}}
            }).Measurements.Select(x => new SelectListItem{Text = x.Name, Value = x.Id.ToString()}).ToList();

            viewModel.OutputCategories = _outputCategoryService.GetOutputCategories(new GetOutputCategoriesRequest
            {
                Take = -1,
                SortingDictionary = new SortedDictionary<string, SortOrder> {{"Order", SortOrder.Ascending} }
            }).OutputCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            foreach (var name in Enum.GetNames(typeof(Formula)))
            {
                viewModel.Formulas.Add(new SelectListItem { Text = name, Value = name });
            }
            viewModel.IsActive = true;
            return View(viewModel);
        }
        
        [HttpPost]
        public ActionResult Create(OutputConfigViewModel viewModel) {
            return View();
        }

        public ActionResult EconomicKpis(string term) {
            return Json(new { results = _outputConfigService.GetKpis(new GetKpisRequest { Term = term }).KpiList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult KeyAssumptions(string term) {
            return Json(new { results = _outputConfigService.GetKeyAssumptions(new GetKeyAssumptionsRequest { Term = term }).KeyAssumptions }, JsonRequestBehavior.AllowGet);
        }
	}
}