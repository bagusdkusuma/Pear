using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightGroup;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.HighlightGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class HighlightGroupController : BaseController
    {
        private IHighlightGroupService _highlightGroupService;
        public HighlightGroupController(IHighlightGroupService highlightGroupService) {
            _highlightGroupService = highlightGroupService;
        }

        public ActionResult Index() {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var templates = _highlightGroupService.GetHighlightGroups(new GetHighlightGroupsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.HighlightGroups.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.HighlightGroups
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HighlightGroupViewModel viewModel) {
            var req = viewModel.MapTo<SaveHighlightGroupRequest>();
            var resp = _highlightGroupService.Save(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id)
        {
            var resp = _highlightGroupService.GetHighlightGroup(new GetHighlightGroupRequest { Id = id });
            return View(resp.MapTo<HighlightGroupViewModel>());
        }

        [HttpPost]
        public ActionResult Edit(HighlightGroupViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightGroupRequest>();
            var resp = _highlightGroupService.Save(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _highlightGroupService.DeleteHighlightGroup(new DeleteHighlightGroupRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}