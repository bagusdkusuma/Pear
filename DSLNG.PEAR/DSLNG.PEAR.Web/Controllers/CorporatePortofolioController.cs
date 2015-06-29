﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Web.ViewModels.CorporatePortofolio;

namespace DSLNG.PEAR.Web.Controllers
{
    public class CorporatePortofolioController : BaseController
    {
        private readonly IPmsSummaryService _pmsSummaryService;

        public CorporatePortofolioController(IPmsSummaryService pmsSummaryService)
        {
            _pmsSummaryService = pmsSummaryService;
        }

        public ActionResult Index()
        {
            var response = _pmsSummaryService.GetPmsSummaryList(new GetPmsSummaryListRequest());
            if (response.IsSuccess)
            {
                var viewModel = new IndexCorporatePortofolioViewModel();
                viewModel.CorporatePortofolios =
                    response.PmsSummaryList.MapTo<IndexCorporatePortofolioViewModel.CorporatePortofolio>();
                return View(viewModel);    
            }

            return base.ErrorPage(response.Message);
        }

        public ActionResult Details(int id)
        {
            return View("Details");
        }

        public ActionResult PmsSummaryConfiguration(int id)
        {
            var response = _pmsSummaryService.GetPmsSummaryConfiguration(new GetPmsSummaryConfigurationRequest {Id = id});
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<PmsSummaryConfigurationViewModel>();
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);
        }
	}
}