
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Web.ViewModels.Weather;
using System.Web.Mvc;
using System.Linq;
using System;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Common.Contants;
using System.Globalization;

namespace DSLNG.PEAR.Web.Controllers
{
    public class WeatherController : BaseController
    {
        private readonly IWeatherService _weatherService;
        private readonly ISelectService _selectService;

        public WeatherController(IWeatherService weatherService,ISelectService selectService) {
            _weatherService = weatherService;
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_IndexGridPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("PeriodeType");
            viewModel.Columns.Add("Date");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("Temperature");
           
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridHighlightIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _weatherService.GetWeathers(new GetWeathersRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _weatherService.GetWeathers(new GetWeathersRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Weathers;
        }

        public ActionResult Create() {
            var viewModel = new WeatherViewModel();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(WeatherViewModel viewModel) {
            var request = viewModel.MapTo<SaveWeatherRequest>();
            _weatherService.SaveWeather(request);
            return RedirectToAction("Index");
        }

        public ActionResult Manage()
        {
            var viewModel = new WeatherViewModel();
            var id = string.IsNullOrEmpty(Request.QueryString["WeatherId"]) ? 0 : int.Parse(Request.QueryString["WeatherId"]);
            if (id != 0)
            {
                viewModel = _weatherService.GetWeather(new GetWeatherRequest { Id = id }).MapTo<WeatherViewModel>();
            }
            else {
                viewModel.PeriodeType = string.IsNullOrEmpty(Request.QueryString["PeriodeType"]) ? "Daily"
                    : Request.QueryString["PeriodeType"];
                var periodeQS = !string.IsNullOrEmpty(Request.QueryString["Periode"]) ? Request.QueryString["Periode"] : null;
                switch (viewModel.PeriodeType)
                {
                    case "Monthly":
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact("01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                    case "Yearly":
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact("01/01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                    default:
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact(periodeQS, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                }
            }
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Manage(WeatherViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveWeatherRequest>();
            _weatherService.SaveWeather(request);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var viewModel = _weatherService.GetWeather(new GetWeatherRequest { Id = id }).MapTo<WeatherViewModel>();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(WeatherViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveWeatherRequest>();
            _weatherService.SaveWeather(request);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            _weatherService.Delete(new DeleteWeatherRequest { Id = id });
            return RedirectToAction("Index");
        }


        public ActionResult Grid(GridParams gridParams)
        {
            var weather = _weatherService.GetWeathersForGrid(new GetWeathersRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = weather.TotalRecords,
                iTotalRecords = weather.Weathers.Count,
                aaData = weather.Weathers.Select(x => new
                {
                    x.Id,
                    PeriodeType = x.PeriodeType.ToString(),
                    Date = x.Date.ToString(DateFormat.DateForGrid),
                    x.Value,
                    x.Temperature
                })
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
