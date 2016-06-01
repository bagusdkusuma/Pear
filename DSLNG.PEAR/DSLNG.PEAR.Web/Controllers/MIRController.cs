using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.Files;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DevExpress.Web.Mvc;
using DevExpress.Web;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MIRController : BaseController
    {
        private readonly IFileRepositoryService _fileRepositoryService;
        private readonly IDropdownService _dropDownService;
        public MIRController(IFileRepositoryService fileRepostoryService, IDropdownService dropDownService)
        {
            _fileRepositoryService = fileRepostoryService;
            _dropDownService = dropDownService;
        }
        // GET: MIR
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var mirData = _fileRepositoryService.GetFiles(new Services.Requests.Files.GetFilesRequest
            {
                Take = gridParams.DisplayLength,
                Skip = gridParams.DisplayStart,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });
            IList<GetFilesRepositoryResponse.FileRepository> DataResponse = mirData.FileRepositories;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = mirData.TotalRecords,
                iTotalRecords = mirData.FileRepositories.Count,
                aaData = DataResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(AccessLevel = "AllowCreate")]
        public ActionResult Create()
        {
            FileRepositoryCreateViewModel model = new FileRepositoryCreateViewModel
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                Years = _dropDownService.GetYears().MapTo<SelectListItem>(),
                Months = _dropDownService.GetMonths().MapTo<SelectListItem>()
            };
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(FileRepositoryViewModel model)
        {
            return View(model);
        }

        public ActionResult UploadControlCallbackAction()
        {
            UploadControlExtension.GetUploadedFiles("mirUpload", MIRUploadControlSettings.ValidationSettings, MIRUploadControlSettings.FileUploadComplete);
            return null;
        }
    }

    public class MIRUploadControlSettings{
        public static UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".pdf" },
            MaxFileSize = 4194304
        };
        public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                var uploaded = new
                {
                    fileName = e.UploadedFile.FileName,
                    data = e.UploadedFile.FileBytes
                };
                HttpContext.Current.Session["MirAttachment"] = uploaded;
            }
        }
    }
}