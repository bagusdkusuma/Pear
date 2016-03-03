using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PopDashboard;
using DSLNG.PEAR.Services.Responses.PopDashboard;
using DSLNG.PEAR.Web.Grid;
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
    public class PopDashboardController : BaseController
    {
        private readonly IPopDashboardService _popDashboardService;
        private readonly IPopInformationService _popInformationService;
        private readonly IDropdownService _dropdownService;
        public PopDashboardController(IPopDashboardService popDashboardService, IPopInformationService popInformationService, IDropdownService dropdownService)
        {
            _popDashboardService = popDashboardService;
            _popInformationService = popInformationService;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var popDashboard = _popDashboardService.GetPopDashboards(new GetPopDashboardsRequest
            {
                Take = gridParams.DisplayLength,
                Skip = gridParams.DisplayStart,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            IList<GetPopDashboardsResponse.PopDashboard> DatasResponse = popDashboard.PopDashboards;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = popDashboard.TotalRecords,
                iTotalRecords = popDashboard.PopDashboards.Count,
                aaData = DatasResponse
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var viewModel = new SavePopDashboardViewModel();
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Create(SavePopDashboardViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePopDashboardRequest>();
            var response = _popDashboardService.SavePopDashboard(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }


        public ActionResult PopInformation(int id)
        {
            var viewModel = _popDashboardService.GetPopDashboard(new GetPopDashboardRequest { Id = id }).MapTo<GetPopDashboardViewModel>();
            viewModel.Users = _dropdownService.GetUsers().MapTo<SelectListItem>();
            return View(viewModel);
        }
    }
}