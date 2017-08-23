using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Requests.InputData;
using DSLNG.PEAR.Services.Responses.InputData;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.Helpers;
using DSLNG.PEAR.Web.ViewModels.DerTransaction;
using DSLNG.PEAR.Web.ViewModels.InputData;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private IDerTransactionService _derTransactionService;

        public InputDataController(IInputDataService inputDataService, IDropdownService dropdownService, IKpiService kpiService,
            IDerTransactionService derTransactionService)
        {
            _inputDataService = inputDataService;
            _dropdownService = dropdownService;
            _kpiService = kpiService;
            _derTransactionService = derTransactionService;
        }
        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Index()
        {   
            var viewModel = new IndexInputDataViewModel();            
            viewModel.InputDatas = _inputDataService.GetInputDatas().InputDatas.MapTo<IndexInputDataViewModel.InputDataViewModel>();
            ViewBag.Role = UserProfile().RoleName;
            ViewBag.IsSuperAdmin = UserProfile().IsSuperAdmin;
            return View(viewModel);
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
            viewModel.GroupInputDatas = new List<CreateInputDataViewModel.GroupInputData>();
            viewModel.Kpis = _kpiService.GetKpis(new Services.Requests.Kpi.GetKpisRequest { }).Kpis.MapTo<CreateInputDataViewModel.Kpi>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateInputDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOrUpdateInputDataRequest>();
            request.ControllerName = "Input Data";
            request.ActionName = "Create";
            request.UserId = this.UserProfile().UserId;
            request.UpdatedById = UserProfile().UserId;            
            var response = _inputDataService.SaveOrUpdateInputData(request);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
             var response = _inputDataService.Delete(new DeleteInputDataRequest {
                 Id = id,
                 ControllerName = "Input Data",
                 ActionName = "Delete",
                 UserId = this.UserProfile().UserId
             });
            @TempData["IsSuccess"] = response.IsSuccess;
            @TempData["Message"] = response.Message;
            return Redirect("Index");
        }
        public ActionResult Update(int Id)
        {
            var response = _inputDataService.GetInputData(Id);
            if(response.IsSuccess)
            {
                var viewModel = response.MapTo<CreateInputDataViewModel>();
                viewModel.Accountabilities = _dropdownService.GetRoleGroups().MapTo<SelectListItem>();
                viewModel.PeriodeTypes = _dropdownService.GetPeriodeTypesDailyMonthlyYearly().MapTo<SelectListItem>();
                viewModel.Kpis = _kpiService.GetKpis(new Services.Requests.Kpi.GetKpisRequest { }).Kpis.MapTo<CreateInputDataViewModel.Kpi>();
                return View(viewModel);
            }

            return base.ErrorPage("Error when loaded input data");            
        }

        [HttpPost]
        public ActionResult Update(CreateInputDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOrUpdateInputDataRequest>();
            request.UpdatedById = UserProfile().UserId;
            request.ControllerName = "Input Data";
            request.ActionName = "Update";
            request.UserId = UserProfile().UserId;
            var response = _inputDataService.SaveOrUpdateInputData(request);
            return RedirectToAction("Index");
        }
        
        public ActionResult FormInputData(int id, string date)
        {            
            var inputData = _inputDataService.GetInputData(id);

            DateTime theDate = DateTimeHelper.Parse(inputData.PeriodeType, date);
            //if (!string.IsNullOrEmpty(date)) theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            FormInputDataViewModel viewModel = inputData.MapTo<FormInputDataViewModel>();
            viewModel.Date = theDate;
            IList<int> kpiIds = new List<int>();
            foreach (var group in viewModel.GroupInputDatas)
            {
                foreach(var item in group.InputDataKpiAndOrders)
                {
                    if(!kpiIds.Contains(item.KpiId)) kpiIds.Add(item.KpiId);
                }
            }
            
            viewModel.KpiInformationValues = GetKpisValue(theDate, kpiIds.ToArray(), new int[] { });
            ViewBag.Title = inputData.Name;
            viewModel.Id = id;
            return View(viewModel);
        }

        private IList<DerValuesViewModel.KpiInformationValuesViewModel> GetKpisValue(DateTime date, int[] actualKpiIds, int[] targetKpiIds)
        {
            var kpiInformationValuesRequest = new GetKpiInformationValuesRequest
            {
                Date = date,
                ActualKpiIds = actualKpiIds,
                TargetKpiIds = targetKpiIds
            };
            var kpiInformationValuesResponse = _derTransactionService.GetKpiInformationValues(kpiInformationValuesRequest);
            return kpiInformationValuesResponse.KpiInformations.MapTo<DerValuesViewModel.KpiInformationValuesViewModel>();
        }
    }
}