
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Requests.Pillar;
using DSLNG.PEAR.Services.Requests.Template;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.Template;
using DSLNG.PEAR.Common.Extensions;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.Select;
using System.Linq;
using DSLNG.PEAR.Web.Attributes;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class TemplateController : BaseController
    {
        private readonly IArtifactService _artifactService;
        private readonly ITemplateService _templateService;
        private readonly ISelectService _selectService;

        public TemplateController(IArtifactService artifactService, ITemplateService templateService, ISelectService selectService) {
            _artifactService = artifactService;
            _templateService = templateService;
            _selectService = selectService;
        }

        public ActionResult ArtifactList(string term)
        {
            var artifacts = _artifactService.GetArtifactsToSelect(new GetArtifactsToSelectRequest { Term = term }).Artifacts;
            return Json(new { results = artifacts }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeUser(AccessLevel ="AllowView")]
        public ActionResult View(int id)
        {
            ViewData["privileges"] = HttpContext.Session["Template"];
            var template = _templateService.GetTemplate(new GetTemplateRequest{Id = id});
            var viewModel = template.MapTo<TemplateViewModel>();
            return View(viewModel);
        }

        [AuthorizeUser(AccessLevel = "AllowCreate")]
        public ActionResult Create()
        {
            var viewModel = new TemplateViewModel();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly") && !name.Equals("Itd"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            var forbidenTypes = new string[] { "Alert" };
            viewModel.HighlightTypes = _selectService.GetSelect(new GetSelectRequest { Name = "highlight-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() }).Where(x => !x.Text.ToLower().Contains("indicator") 
                && !forbidenTypes.Contains(x.Text) 
                && !x.Text.ToLower().Contains("edg")
                && !x.Text.ToLower().Contains("gtg")).ToList();
            foreach (var name in Enum.GetNames(typeof(TemplateColumnType)))
            {
                viewModel.ColumnTypes.Add(new SelectListItem { Text = name, Value = name });
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(TemplateViewModel viewModel)
        {
            _templateService.CreateTemplate(viewModel.MapTo<CreateTemplateRequest>());
            return RedirectToAction("Index");
        }

        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Update(int id)
        {
            var template = _templateService.GetTemplate(new GetTemplateRequest {Id = id});
            var viewModel = template.MapTo<TemplateViewModel>();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly") && !name.Equals("Itd"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            var forbidenTypes = new string[] { "Alert" };
            viewModel.HighlightTypes = _selectService.GetSelect(new GetSelectRequest { Name = "highlight-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() }).Where(x => !x.Text.ToLower().Contains("indicator")
                && !forbidenTypes.Contains(x.Text)
                && !x.Text.ToLower().Contains("edg")
                && !x.Text.ToLower().Contains("gtg")).ToList();
            foreach (var name in Enum.GetNames(typeof(TemplateColumnType)))
            {
                viewModel.ColumnTypes.Add(new SelectListItem { Text = name, Value = name });
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(TemplateViewModel viewModel)
        {
            try
            {
                var request = viewModel.MapTo<UpdateTemplateRequest>();
                var response = _templateService.UpdateTemplate(request);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Preview(TemplateViewModel viewModel)
        {
            return View(viewModel);
        }



        //
        // POST: /Template/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _templateService.DeleteTemplate(new DeleteTemplateRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");

        }

        public ActionResult Grid(GridParams gridParams)
        {
            var templates = _templateService.GetTemplates(new GetTemplatesRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.Artifacts.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.Artifacts
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        #region Dev Express Grid
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridTemplateIndex");
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
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Remark");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridTemplateIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _templateService.GetTemplates(new GetTemplatesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _templateService.GetTemplates(new GetTemplatesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Artifacts;
        }
        #endregion
    }
}
