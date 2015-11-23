using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Web.ViewModels.Select;
using DevExpress.Web.Mvc;
using System;
using DSLNG.PEAR.Data.Enums;
using System.Linq;
using DSLNG.PEAR.Common.Contants;
using System.IO;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class SelectController : BaseController
    {
        private readonly ISelectService _selectService;

        public SelectController(ISelectService selectService)
        {
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridSelectIndex");
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
            viewModel.Columns.Add("Options");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridSelectIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _selectService.GetSelects(new GetSelectsRequest()).Selects.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _selectService.GetSelects(new GetSelectsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Selects;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateSelectViewModel();
            foreach (var name in Enum.GetNames(typeof(SelectType)))
            {
                viewModel.Types.Add(new SelectListItem { Text = name, Value = name });
            }
            viewModel.Parents = _selectService.GetSelects(new GetSelectsRequest())
                .Selects.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name.ToString() }).ToList();
            viewModel.Parents.Insert(0, new SelectListItem { Value = "0", Text = "No Parent" });
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateSelectViewModel viewModel)
        {
          
            var validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };
            var valids = viewModel.Options.Select(x => x.ValueFile != null &&
                validImageTypes.Contains(x.ValueFile.ContentType)).Count();
            if ((SelectType)Enum.Parse(typeof(SelectType), viewModel.Type, true) == SelectType.Image && valids != viewModel.Options.Count)
            {
                TempData["IsSuccess"] = false;
                TempData["Message"] = string.Format(@"Please choose either a GIF, JPG or PNG image");
            }
            else if((SelectType)Enum.Parse(typeof(SelectType), viewModel.Type, true) == SelectType.Image && valids == viewModel.Options.Count)
            {
                foreach (var option in viewModel.Options)
                {
                    if (option.ValueFile != null)
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(option.ValueFile.InputStream, true, true))
                        {
                            if (!Directory.Exists(Server.MapPath(PathConstant.SelectPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(PathConstant.SelectPath));
                            }
                            var imagePath = Path.Combine(Server.MapPath(PathConstant.SelectPath), option.ValueFile.FileName);
                            option.ValueFile.SaveAs(imagePath);
                            option.Value = option.ValueFile.FileName;
                        }
                       

                    }
                }
            }
            var request = viewModel.MapTo<CreateSelectRequest>();
            var response = _selectService.Create(request);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Update(int id)
        {
            var response = _selectService.GetSelect(new GetSelectRequest { Id = id });
            var viewModel = response.MapTo<UpdateSelectViewModel>();
            foreach (var name in Enum.GetNames(typeof(SelectType)))
            {
                viewModel.Types.Add(new SelectListItem { Text = name, Value = name });
            }
            viewModel.Parents = _selectService.GetSelects(new GetSelectsRequest())
               .Selects.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name.ToString() }).ToList();
            if (viewModel.ParentId != 0) {
                viewModel.ParentOptions = response.ParentOptions.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text.ToString() }).ToList();
            }
            viewModel.Parents.Insert(0, new SelectListItem { Value = "0", Text = "No Parent" });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateSelectViewModel viewModel)
        {
            var validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };
            var valids = viewModel.Options.Select(x => x.ValueFile != null &&
                validImageTypes.Contains(x.ValueFile.ContentType)).Count();
            if ((SelectType)Enum.Parse(typeof(SelectType), viewModel.Type, true) == SelectType.Image && valids != viewModel.Options.Count)
            {
                TempData["IsSuccess"] = false;
                TempData["Message"] = string.Format(@"Please choose either a GIF, JPG or PNG image");
            }
            else if ((SelectType)Enum.Parse(typeof(SelectType), viewModel.Type, true) == SelectType.Image && valids == viewModel.Options.Count)
            {
                foreach (var option in viewModel.Options)
                {
                    if (option.ValueFile != null)
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(option.ValueFile.InputStream, true, true))
                        {
                            if (!Directory.Exists(Server.MapPath(PathConstant.SelectPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(PathConstant.SelectPath));
                            }
                            var imagePath = Path.Combine(Server.MapPath(PathConstant.SelectPath), option.ValueFile.FileName);
                            option.ValueFile.SaveAs(imagePath);
                            option.Value = option.ValueFile.FileName;
                        }


                    }
                }
            }
            var request = viewModel.MapTo<UpdateSelectRequest>();
            var response = _selectService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _selectService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public JsonResult Options(int id) { 
            var select = _selectService.GetSelect(new GetSelectRequest{Id = id});
            return Json(select.Options,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var select = _selectService.GetSelectsForGrid(new GetSelectsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = select.TotalRecords,
                iTotalRecords = select.Selects.Count,
                aaData = select.Selects
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}