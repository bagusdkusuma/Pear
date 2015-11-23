using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.Pillar;
using System.Web.Mvc;
using DSLNG.PEAR.Web.Grid;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Requests.Pillar;
using DSLNG.PEAR.Web.ViewModels.Pillar;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PillarController : BaseController
    {
        private readonly IPillarService _pillarService;

        public PillarController(IPillarService service)
        {
            _pillarService = service;
        }


        public ActionResult Index()
        {

            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridPillarIndex");
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
            return PartialView("_GridViewPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Code");
            viewModel.Columns.Add("Order");
            viewModel.Columns.Add("Color");
            viewModel.Columns.Add("Icon");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridPillarIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _pillarService.GetPillars(new GetPillarsRequest()).Pillars.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _pillarService.GetPillars(new GetPillarsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Pillars;
        }


        public ActionResult Create()
        {
            var viewModel = new CreatePillarViewModel();
            viewModel.Icons = Directory.EnumerateFiles(Server.MapPath(PathConstant.PillarPath)).ToList();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePillarViewModel viewModel)
        {
            var request = viewModel.MapTo<CreatePillarRequest>();
            var response = _pillarService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public ActionResult Update(int id)
        {
            var response = _pillarService.GetPillar(new GetPillarRequest { Id = id });
            var viewModel = response.MapTo<UpdatePillarViewModel>();
            viewModel.Icons = Directory.EnumerateFiles(Server.MapPath(PathConstant.PillarPath)).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdatePillarViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdatePillarRequest>();
            
            /*if (viewModel.IconFile != null)
            {
                if (!validImageTypes.Contains(viewModel.IconFile.ContentType))
                {
                    ModelState.AddModelError("IconFile", "Please choose either a GIF, JPG or PNG image.");
                }
                else
                {
                    var name = Guid.NewGuid() + "_" + viewModel.IconFile.FileName;

                    if (!Directory.Exists(Server.MapPath(PathConstant.PillarPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(PathConstant.PillarPath));
                    }

                    var imagePath = Path.Combine(Server.MapPath(PathConstant.PillarPath), name);
                    //var imageUrl = Path.Combine(UploadDir, name);
                    viewModel.IconFile.SaveAs(imagePath);
                    request.Icon = name;
                }
            }
*/
            if (!ModelState.IsValid)
            {
                return View("Update", viewModel);
            }
            var response = _pillarService.Update(request);
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
            var response = _pillarService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase iconFile, string returnUrl)
        {
            
            var validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };

            if (iconFile != null)
            {
                if (!validImageTypes.Contains(iconFile.ContentType))
                {
                    TempData["IsSuccess"] = false;
                    TempData["Message"] = string.Format(@"Please choose either a GIF, JPG or PNG image");
                }
                else
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(iconFile.InputStream, true, true))
                    {
                        if (image.Width <= ImageConstant.Width && image.Height <= ImageConstant.Height)
                        {
                            if (!Directory.Exists(Server.MapPath(PathConstant.PillarPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(PathConstant.PillarPath));
                            }
                            var imagePath = Path.Combine(Server.MapPath(PathConstant.PillarPath), iconFile.FileName);
                            iconFile.SaveAs(imagePath);
                            TempData["IsSuccess"] = true;
                            TempData["Message"] = "Icon has been uploaded successfully";
                        }
                        else
                        {
                            TempData["IsSuccess"] = false;
                            TempData["Message"] =
                                string.Format(@"The dimensions of image should not be more than {0}x{1} px",
                                              ImageConstant.Width, ImageConstant.Height);
                        }
                    }
                }
            }

            return Redirect(returnUrl);

        }

        public ActionResult DeleteIcon(string name, string redirectAction)
        {
            string fullPath = Request.MapPath(PathConstant.PillarPath + "/" + name);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return RedirectToAction(redirectAction);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var pillars = _pillarService.GetPillars(new GetPillarsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary,
                    Search = gridParams.Search
                });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = pillars.Pillars.Count,
                iTotalDisplayRecords = pillars.TotalRecords,
                aaData = pillars.Pillars
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        
        }
    }
}