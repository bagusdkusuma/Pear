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
            return View(ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }

        [ValidateInput(false)]
        public ActionResult ProcessBlueprintPartial()
        {
            return PartialView("_ProcessBlueprintPartial", ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }

        public FileStreamResult ProcessBlueprintPartialDownload()
        {
            return FileManagerExtension.DownloadFiles(ProcessBlueprintControllerProcessBlueprintSettings.CreateFileManagerDownloadSettings(), ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }
	}
    public class ProcessBlueprintControllerProcessBlueprintSettings
    {
        static ProcessBlueprintFileSystemProvider processBlueprintProvider;
        public static readonly object FileManagerFolder = "~/Content/FileManager";
        public static readonly object RootFolder = "~/Content/FileManager";
        public static readonly object ImagesRootFolder = "~/Content/images";
        public static readonly string[] AllowedFileExtensions = new string[] {
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".avi", ".png", ".mp3", ".xml", ".doc", ".pdf"
        };

        public static ProcessBlueprintFileSystemProvider ProcessBlueprintFileSystemProvider
        {
            get
            {
                if (processBlueprintProvider == null)
                    processBlueprintProvider = new ProcessBlueprintFileSystemProvider(string.Empty);
                return processBlueprintProvider;
            }
        }

        static DevExpress.Web.FileManagerSettingsDataSource _dataSourceSettings;

        public static DevExpress.Web.FileManagerSettingsDataSource DataSourceSettings
        {
            get
            {
                if (_dataSourceSettings == null)
                {
                    _dataSourceSettings = new DevExpress.Web.FileManagerSettingsDataSource();
                    _dataSourceSettings.KeyFieldName = "FileId";
                    _dataSourceSettings.ParentKeyFieldName = "ParentId";
                    _dataSourceSettings.IsFolderFieldName = "IsFolder";
                    _dataSourceSettings.NameFieldName = "Name";
                    _dataSourceSettings.FileBinaryContentFieldName = "Data";
                    _dataSourceSettings.LastWriteTimeFieldName = "LastWriteTime";
                }
                return _dataSourceSettings;
            }
        }

        //public static ProcessBlueprintFileSystemProvider Model { get { return new ProcessBlueprintFileSystemProvider(""); } }
        public static DevExpress.Web.Mvc.MVCxDataSourceFileSystemProvider Model
        {
            get
            {
                object dataModel = new object[0]; // Insert here your data model object
                return new DevExpress.Web.Mvc.MVCxDataSourceFileSystemProvider(dataModel, DataSourceSettings);
            }
        }

        public static DevExpress.Web.Mvc.FileManagerSettings CreateFileManagerDownloadSettings()
        {
            var settings = new DevExpress.Web.Mvc.FileManagerSettings();

            settings.SettingsEditing.AllowDownload = true;

            settings.Name = "ProcessBlueprint";
            return settings;
        }
    }

    //public class ProcessBlueprintControllerProcessBlueprintSettingsCustomFileSystemProvider : DevExpress.Web.FileSystemProviderBase
    //{
    //    public ProcessBlueprintControllerProcessBlueprintSettingsCustomFileSystemProvider(string rootFolder)
    //        : base(rootFolder) { }
    //    public override IEnumerable<DevExpress.Web.FileManagerFile> GetFiles(DevExpress.Web.FileManagerFolder folder)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override IEnumerable<DevExpress.Web.FileManagerFolder> GetFolders(DevExpress.Web.FileManagerFolder parentFolder)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override bool Exists(DevExpress.Web.FileManagerFile file)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override bool Exists(DevExpress.Web.FileManagerFolder folder)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}