using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Wave;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.Wave;
using DSLNG.PEAR.Services.Requests.Select;

namespace DSLNG.PEAR.Web.Controllers
{
    public class WaveController : BaseController
    {
        private readonly IWaveService _waveService;
        private readonly ISelectService _selectService;

        public WaveController(IWaveService waveService, ISelectService selectService)
        {
            _waveService = waveService;
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var waves = _waveService.GetWavesForGrid(new GetWavesRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary
            });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = waves.TotalRecords,
                iTotalRecords = waves.Waves.Count,
                aaData = waves.Waves.Select(x => new
                {
                    x.Id,
                    PeriodeType = x.PeriodeType.ToString(),
                    Date = x.Date.ToString(DateFormat.DateForGrid),
                    x.Value,
                    x.Speed,
                    x.Tide
                })
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var viewModel = new WaveViewModel();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }

            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "wave-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(WaveViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveWaveRequest>();
            _waveService.SaveWave(request);
            return RedirectToAction("Index");
        }
	}
}