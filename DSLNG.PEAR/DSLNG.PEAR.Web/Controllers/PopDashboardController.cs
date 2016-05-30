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
using System.Data.SqlClient;
using System.IO;
using DSLNG.PEAR.Common.Contants;

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
            var StatusList = new List<SelectListItem>() { 
                new SelectListItem{Value = "Not Start Yet", Text = "Not Start Yet"},
                new SelectListItem{Value = "In Progress", Text = "In Progress"},
                new SelectListItem{Value = "Finish", Text = "Finish"}
            };
            viewModel.StatusOptions = StatusList;
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Create(SavePopDashboardViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePopDashboardRequest>();
            ProcessAttachment(viewModel, request);

            var response = _popDashboardService.SavePopDashboard(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(SavePopDashboardViewModel viewModel, HttpPostedFileBase file)
        {
            var request = viewModel.MapTo<SavePopDashboardRequest>();
            ProcessAttachment(viewModel, request);
            var response = _popDashboardService.SavePopDashboard(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var viewModel = _popDashboardService.GetPopDashboard(new GetPopDashboardRequest { Id = id }).MapTo<SavePopDashboardViewModel>();
            if (viewModel.Attachments == null || viewModel.Attachments.Count == 0) {
                viewModel.Attachments = new List<SavePopDashboardViewModel.AttachmentViewModel>{
                    new SavePopDashboardViewModel.AttachmentViewModel()
                };
            }
            var StatusList = new List<SelectListItem>() { 
                new SelectListItem{Value = "Not Start Yet", Text = "Not Start Yet"},
                new SelectListItem{Value = "In Progress", Text = "In Progress"},
                new SelectListItem{Value = "Finish", Text = "Finish"}
            };
            viewModel.StatusOptions = StatusList;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _popDashboardService.DeletePopDashboard(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();

        }


        public ActionResult PopInformation(int id)
        {
            var viewModel = _popDashboardService.GetPopDashboard(new GetPopDashboardRequest { Id = id }).MapTo<GetPopDashboardViewModel>();
            viewModel.Users = _dropdownService.GetUsers().MapTo<SelectListItem>();
            return View(viewModel);
        }

        public ActionResult Approval(int id)
        {
            var viewModel = _popDashboardService.GetPopDashboard(new GetPopDashboardRequest { Id = id }).MapTo<GetPopDashboardViewModel>();
            viewModel.Users = _dropdownService.GetUsers().MapTo<SelectListItem>();
            return View(viewModel);
        }
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
        private void ProcessAttachment(SavePopDashboardViewModel viewModel, SavePopDashboardRequest request)
        {
            if (viewModel.Attachments.Count > 0)
            {
                var validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };
                var pdfType = "application/pdf";
                var excelTypes = new string[]{
                    "application/vnd.ms-excel",
                    "application/msexcel",
                    "application/x-msexcel",
                    "application/x-ms-excel",
                    "application/x-excel",
                    "application/x-dos_ms_excel",
                    "application/xls",
                    "application/x-xls",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                var docTypes = new string[]{
                    "application/msword",
                    "application/msword",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                    "application/vnd.ms-word.document.macroEnabled.12",
                    "application/vnd.ms-word.template.macroEnabled.12"
                };
                var pptTypes = new string[]{
                   "application/vnd.ms-powerpoint",
                    "application/vnd.ms-powerpoint",
                    "application/vnd.ms-powerpoint",
                    "application/vnd.ms-powerpoint",
                    "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                    "application/vnd.openxmlformats-officedocument.presentationml.template",
                    "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                    "application/vnd.ms-powerpoint.addin.macroEnabled.12",
                    "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
                    "application/vnd.ms-powerpoint.template.macroEnabled.12",
                    "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"
                };
                foreach (var attachment in viewModel.Attachments)
                {
                    if (attachment.File != null)
                    {
                        //var filename = Path.GetFileName(attachment.File.FileName);
                        string type = null;
                        if (attachment.File.ContentType == pdfType)
                        {
                            type = "pdf";
                        }
                        else if (validImageTypes.Contains(attachment.File.ContentType))
                        {
                            type = "image";
                        }
                        else if (excelTypes.Contains(attachment.File.ContentType))
                        {
                            type = "excel";
                        }
                        else if (docTypes.Contains(attachment.File.ContentType))
                        {
                            type = "doc";
                        }
                        else if (pptTypes.Contains(attachment.File.ContentType))
                        {
                            type = "ppt";
                        }
                        else
                        {
                            type = "blank";
                        }
                        if (!Directory.Exists(Server.MapPath(PathConstant.PopAttachmentPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(PathConstant.PopAttachmentPath));
                        }
                        var filename = attachment.File.FileName;
                        var uniqueFilename = RandomString(8) + MakeValidFileName(attachment.File.FileName).Replace(" ", "_");
                        var filePath = Path.Combine(Server.MapPath(PathConstant.PopAttachmentPath), uniqueFilename);
                        var url = PathConstant.PopAttachmentPath + "/" + uniqueFilename;
                        attachment.File.SaveAs(filePath);
                        var attachmentReq = new SavePopDashboardRequest.Attachment
                        {
                            Id = attachment.Id,
                            FileName = url,
                            Alias = string.IsNullOrEmpty(attachment.Alias) ? filename : attachment.Alias,
                            Type = type
                        };
                        request.AttachmentFiles.Add(attachmentReq);
                    }
                    else
                    {
                        if (attachment.Id != 0)
                        {
                            var attachmentReq = new SavePopDashboardRequest.Attachment
                            {
                                Id = attachment.Id,
                                Alias = attachment.Alias,
                            };
                            request.AttachmentFiles.Add(attachmentReq);
                        }
                    }
                }
            }
        }
    }
}