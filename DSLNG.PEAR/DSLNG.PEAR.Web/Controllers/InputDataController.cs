using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.InputData;
using DSLNG.PEAR.Services.Responses.InputData;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class InputDataController : BaseController
    {
        private IInputDataService _inputDataService;
        private IDropdownService _dropdownService;
        private IKpiService _kpiService;

        public InputDataController(IInputDataService inputDataService, IDropdownService dropdownService, IKpiService kpiService)
        {
            _inputDataService = inputDataService;
            _dropdownService = dropdownService;
            _kpiService = kpiService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var inputDatas = _inputDataService.GetInputData(new GetInputDatasRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });
            inputDatas.InputDatas.Add(new GetInputDatasResponse.InputData { Name = "Input Data DER", PeriodeType = "Daily" });

            IList<GetInputDatasResponse.InputData> inputDatasResponse = inputDatas.InputDatas;            
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = inputDatas.TotalRecords + 1,
                iTotalRecords = inputDatas.InputDatas.Count,
                aaData = inputDatasResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var viewModel = new CreateInputDataViewModel();
            viewModel.Accountabilities = _dropdownService.GetRoleGroups().MapTo<SelectListItem>();
            viewModel.PeriodeTypes = _dropdownService.GetPeriodeTypesDailyMonthlyYearly().MapTo<SelectListItem>();
            viewModel.GroupInputs = new List<CreateInputDataViewModel.GroupInput>();
            viewModel.Kpis = _kpiService.GetKpis(new Services.Requests.Kpi.GetKpisRequest { }).Kpis.MapTo<CreateInputDataViewModel.Kpi>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateInputDataViewModel viewModel, FormCollection form)
        {
            
        }
    }
}