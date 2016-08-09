using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.DerTransaction;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerTransactionController : BaseController
    {
        private readonly IDerService _derService;
        private readonly IDerTransactionService _derTransactionService;

        public DerTransactionController(IDerService derService,IDerTransactionService derTransactionService) {
            _derService = derService;
            _derTransactionService = derTransactionService;
        }
        // GET: DerTransaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Input()
        {
            return View();
        }

        public ActionResult EconomicIndicator(string date)
        {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var previousDate = theDate.AddDays(-1);
            var request = new GetDerLayoutItemsRequest {
                LayoutId = _derService.GetActiveDer().Id,
                Positions = new List<GetDerLayoutItemsRequest.Position> {
                    new GetDerLayoutItemsRequest.Position { Row = 14, Column = 1},
                    new GetDerLayoutItemsRequest.Position {Row= 16, Column=1 },
                    new GetDerLayoutItemsRequest.Position {Row= 15, Column=1 },
                    new GetDerLayoutItemsRequest.Position {Row= 15, Column=2 }
                },
                DerLayoutItemTypes = new List<DerLayoutItemType> {
                    DerLayoutItemType.KpiInformations,
                    DerLayoutItemType.Highlight
                }
            };
            var layoutItems = _derTransactionService.GetDerLayoutItems(request);
            var kpiInformationValuesRequest = new GetKpiInformationValuesRequest {
                Date = theDate,
                KpiInformations = layoutItems.DerLayoutItems.SelectMany(x => x.KpiInformations).ToList()
            };
            var kpiInformationValuesResponse = _derTransactionService.GetKpiInformationValues(kpiInformationValuesRequest);
            var highlightValuesRequest = new GetHighlightValuesRequest {
                Date = theDate,
                DerHighlights = layoutItems.DerLayoutItems.Where(x => x.Highlight != null).Select(x => x.Highlight).ToList()
            };
            var highlightValuesResponse = _derTransactionService.GetHighlightValues(highlightValuesRequest);
            var viewModel = layoutItems.MapTo<DerValuesViewModel>();
            foreach (var layoutItem in viewModel.DerLayoutItems) {
                foreach (var kpiInformation in layoutItem.KpiInformations) {
                    var kpiInformationValue = kpiInformationValuesResponse.KpiInformations.FirstOrDefault(x => x.KpiId == kpiInformation.KpiId);
                    if(kpiInformationValue != null)
                        kpiInformationValue.MapPropertiesToInstance(kpiInformation);
                }
                if (layoutItem.Highlight != null)
                {
                    var highlightValue = highlightValuesResponse.Highlights.FirstOrDefault(x => x.HighlightTypeId == layoutItem.Highlight.HighlightTypeId);
                    if (highlightValue != null)
                        highlightValue.MapPropertiesToInstance(layoutItem.Highlight);
                }
            }
            return View(viewModel);
        }

        public ActionResult ForcastedIndicator(string date) {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var previousDate = theDate.AddDays(-1);
            var request = new GetDerLayoutItemsRequest
            {
                LayoutId = _derService.GetActiveDer().Id,
                Positions = new List<GetDerLayoutItemsRequest.Position> {
                    new GetDerLayoutItemsRequest.Position {Row= 16, Column=1 }
                },
                DerLayoutItemTypes = new List<DerLayoutItemType> {
                    DerLayoutItemType.KpiInformations,
                    DerLayoutItemType.Highlight
                }
            };
            var layoutItems = _derTransactionService.GetDerLayoutItems(request);
            var kpiInformationValuesRequest = new GetKpiInformationValuesRequest
            {
                Date = theDate,
                KpiInformations = layoutItems.DerLayoutItems.SelectMany(x => x.KpiInformations).ToList()
            };
            var kpiInformationValuesResponse = _derTransactionService.GetKpiInformationValues(kpiInformationValuesRequest);
            var highlightValuesRequest = new GetHighlightValuesRequest
            {
                Date = theDate,
                DerHighlights = layoutItems.DerLayoutItems.Where(x => x.Highlight != null).Select(x => x.Highlight).ToList()
            };
            var highlightValuesResponse = _derTransactionService.GetHighlightValues(highlightValuesRequest);
            var viewModel = layoutItems.MapTo<DerValuesViewModel>();
            foreach (var layoutItem in viewModel.DerLayoutItems)
            {
                foreach (var kpiInformation in layoutItem.KpiInformations)
                {
                    var kpiInformationValue = kpiInformationValuesResponse.KpiInformations.FirstOrDefault(x => x.KpiId == kpiInformation.KpiId);
                    if (kpiInformationValue != null)
                        kpiInformationValue.MapPropertiesToInstance(kpiInformation);
                }
                if (layoutItem.Highlight != null)
                {
                    var highlightValue = highlightValuesResponse.Highlights.FirstOrDefault(x => x.HighlightTypeId == layoutItem.Highlight.HighlightTypeId);
                    if (highlightValue != null)
                        highlightValue.MapPropertiesToInstance(layoutItem.Highlight);
                }
            }
            return View(viewModel);
        }

        public ActionResult OperationSection(string date) {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var previousDate = theDate.AddDays(-1);
            var request = new GetDerLayoutItemsRequest
            {
                LayoutId = _derService.GetActiveDer().Id,
                Positions = new List<GetDerLayoutItemsRequest.Position> {
                    new GetDerLayoutItemsRequest.Position {Row= 6, Column=2 },
                    new GetDerLayoutItemsRequest.Position {Row= 5, Column=2 },
                    new GetDerLayoutItemsRequest.Position {Row= 5, Column = 0},
                    new GetDerLayoutItemsRequest.Position {Row=5, Column = 1},
                    new GetDerLayoutItemsRequest.Position {Row = 7, Column = 0}
                },
                DerLayoutItemTypes = new List<DerLayoutItemType> {
                    DerLayoutItemType.KpiInformations,
                    DerLayoutItemType.Highlight
                }
            };
            var layoutItems = _derTransactionService.GetDerLayoutItems(request);
            var kpiInformationValuesRequest = new GetKpiInformationValuesRequest
            {
                Date = theDate,
                KpiInformations = layoutItems.DerLayoutItems.SelectMany(x => x.KpiInformations).ToList()
            };
            var kpiInformationValuesResponse = _derTransactionService.GetKpiInformationValues(kpiInformationValuesRequest);
            var highlightValuesRequest = new GetHighlightValuesRequest
            {
                Date = theDate,
                DerHighlights = layoutItems.DerLayoutItems.Where(x => x.Highlight != null).Select(x => x.Highlight).ToList()
            };
            var highlightValuesResponse = _derTransactionService.GetHighlightValues(highlightValuesRequest);
            var viewModel = layoutItems.MapTo<DerValuesViewModel>();
            foreach (var layoutItem in viewModel.DerLayoutItems)
            {
                foreach (var kpiInformation in layoutItem.KpiInformations)
                {
                    var kpiInformationValue = kpiInformationValuesResponse.KpiInformations.FirstOrDefault(x => x.KpiId == kpiInformation.KpiId);
                    if (kpiInformationValue != null)
                        kpiInformationValue.MapPropertiesToInstance(kpiInformation);
                }
                if (layoutItem.Highlight != null)
                {
                    var highlightValue = highlightValuesResponse.Highlights.FirstOrDefault(x => x.HighlightTypeId == layoutItem.Highlight.HighlightTypeId);
                    if (highlightValue != null)
                        highlightValue.MapPropertiesToInstance(layoutItem.Highlight);
                }
            }
            return View(viewModel);
        }
    }
}