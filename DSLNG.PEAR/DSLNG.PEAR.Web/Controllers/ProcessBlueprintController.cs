using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.ProcessBlueprint;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ProcessBlueprintController : Controller
    {
        //
        // GET: /ProcessBlueprint/
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ProcessBlueprintPartial()
        {
            return PartialView("_ProcessBlueprintPartial", ProcessBlueprintControllerProcessBlueprintSettings.Model);
        }

        public FileStreamResult ProcessBlueprintPartialDownload()
        {
            return FileManagerExtension.DownloadFiles("ProcessBlueprint", ProcessBlueprintControllerProcessBlueprintSettings.Model);
        }
	}
    public class ProcessBlueprintControllerProcessBlueprintSettings
    {
        public static readonly object FileManagerFolder = "~/Content/FileManager";
        public static readonly object RootFolder = "~/Content/FileManager";
        public static readonly object ImagesRootFolder = "~/Content/images";
        public static readonly string[] AllowedFileExtensions = new string[] {
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".avi", ".png", ".mp3", ".xml", ".doc", ".pdf"
        };

        public static ProcessBlueprintFileSystemProvider Model { get { return new ProcessBlueprintFileSystemProvider(""); } }
    }

    public class ProcessBlueprintControllerProcessBlueprintSettingsCustomFileSystemProvider : DevExpress.Web.FileSystemProviderBase
    {
        public ProcessBlueprintControllerProcessBlueprintSettingsCustomFileSystemProvider(string rootFolder)
            : base(rootFolder) { }
        public override IEnumerable<DevExpress.Web.FileManagerFile> GetFiles(DevExpress.Web.FileManagerFolder folder)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<DevExpress.Web.FileManagerFolder> GetFolders(DevExpress.Web.FileManagerFolder parentFolder)
        {
            throw new NotImplementedException();
        }
        public override bool Exists(DevExpress.Web.FileManagerFile file)
        {
            throw new NotImplementedException();
        }
        public override bool Exists(DevExpress.Web.FileManagerFolder folder)
        {
            throw new NotImplementedException();
        }
    }

}