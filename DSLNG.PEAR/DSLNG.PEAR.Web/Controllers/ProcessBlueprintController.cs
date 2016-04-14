using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.ProcessBlueprint;
using DSLNG.PEAR.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using DevExpress.Web;
using Newtonsoft.Json;
using System.IO;

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
        public ActionResult ProcessBlueprintPartial([Bind]FileManagerFeaturesOption options)
        {
            string selectedFolder = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["ProcessBlueprint_State"]))
            {
                dynamic state = JsonConvert.DeserializeObject(Request.Params["ProcessBlueprint_State"]);
                selectedFolder = (string)state.currentPath.Value;
            }
            var provider = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider;
            var folder = new FileManagerFolder(provider, selectedFolder);
            ProcessBlueprintControllerProcessBlueprintSettings.FeatureOptions = options;
            lock (ProcessBlueprintControllerProcessBlueprintSettings.SettingsPermissions)
            {
                ProcessBlueprintControllerProcessBlueprintSettings.SettingsPermissions.AccessRules.Clear();
                ProcessBlueprintControllerProcessBlueprintSettings.ApplyRules(folder);
            }
            return PartialView("_ProcessBlueprintPartial", ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }

        public PartialViewResult PrivilegeViewPartialView(FileSystemItem model)
        {
            ///todo create matrix of file vs role vs privilege
            return PartialView("_PrivilegePartial");
        }
        public ActionResult ProcessConfigPartial(string relativePath)
        {
            string selecetedFile = string.Empty;
            FileSystemItem item = new FileSystemItem();
            if (!string.IsNullOrEmpty(relativePath))
            {
                string[] val = relativePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                for (int i = 0; i < val.Length; i++)
                {
                    selecetedFile = Path.Combine(selecetedFile, val[i]);
                }
                var provider = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider;
                var file = new FileManagerFile(provider, selecetedFile);
                item = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider.GetFile(file);
            }
            //FileManagerFile file = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider.GetFile()


            return PartialView("_PrivilegeConfigPartial", item);
        }
        public FileStreamResult ProcessBlueprintPartialDownload()
        {
            return FileManagerExtension.DownloadFiles(ProcessBlueprintControllerProcessBlueprintSettings.CreateFileManagerDownloadSettings(), ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }
	}

    #region FileManager Settings
    public class ProcessBlueprintControllerProcessBlueprintSettings
    {
        static ProcessBlueprintFileSystemProvider processBlueprintProvider;
        public static readonly object FileManagerFolder = "~/Content/FileManager";
        public static readonly object RootFolder = "~/Content/FileManager";
        public static readonly object ImagesRootFolder = "~/Content/images";
        public static readonly string[] AllowedFileExtensions = new string[] {
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".avi", ".png", ".mp3", ".xml", ".doc", ".pdf"
        };

        static UserProfileSessionData sessionData = (UserProfileSessionData)HttpContext.Current.Session["LoginUser"];

        static FileManagerSettingsPermissions settings = new FileManagerSettingsPermissions(null);
        public static FileManagerSettingsPermissions SettingsPermissions { get { return settings; } }
        public static void ApplyRules(FileManagerFolder folder)
        {

            //set for my own files
            var myFolder = ProcessBlueprintDataProvider.GetAll().FindAll(x => x.CreatedBy == sessionData.UserId).ToList();
            foreach (var item in myFolder)
            {
                FileManagerAccessRuleBase rule = null;
                // get folderitem
                var folderItem = ProcessBlueprintFileSystemProvider.GetRelativeName(item);
                if (item.IsFolder)
                {
                    rule = new FileManagerFolderAccessRule();
                    //settings.AccessRules.Add(new FileManagerFolderAccessRule(folderItem) { Edit = Rights.Allow, Browse = Rights.Allow, Role = sessionData.RoleName });
                    //settings.AccessRules.Add(new FileManagerFolderAccessRule(folderItem) { EditContents = Rights.Allow, Role = sessionData.RoleName });
                }
                else
                {
                    rule = new FileManagerFileAccessRule();
                    //settings.AccessRules.Add(new FileManagerFileAccessRule(folderItem) { Edit = Rights.Allow, Role = sessionData.RoleName });
                }
                rule.Path = folderItem;
                rule.Browse = Rights.Allow;
                rule.Edit = Rights.Allow;
                settings.AccessRules.Add(rule);
            }
        }
        public static FileManagerFeaturesOption FeatureOptions
        {
            get
            {
                if (HttpContext.Current.Session["FeatureOptions"] == null)
                {
                    HttpContext.Current.Session["FeatureOptions"] = new FileManagerFeaturesOption();
                }
                return (FileManagerFeaturesOption)HttpContext.Current.Session["FeatureOptions"];
            }
            set
            {
                HttpContext.Current.Session["FeatureOptions"] = value;
            }
        }
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
                //object dataModel = new list<FileSystemItem>(); // Insert here your data model object
                List<FileSystemItem> datas = ProcessBlueprintDataProvider.GetAll();

                return new DevExpress.Web.Mvc.MVCxDataSourceFileSystemProvider(datas, DataSourceSettings);
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
    #endregion

    #region FileManager Option
    public class FileManagerFeaturesOption
    {
        FileManagerSettingsEditing settingsEditing;
        FileManagerSettingsToolbar settingsToolbar;
        FileManagerSettingsFolders settingsFolders;
        MVCxFileManagerSettingsUpload settingsUpload;
        UserProfileSessionData sessionData = (UserProfileSessionData)HttpContext.Current.Session["LoginUser"];
        public FileManagerFeaturesOption()
        {
            FileManagerPrivilege privileges = GetPrivilege(sessionData);

            this.settingsEditing = new FileManagerSettingsEditing(null)
            {
                AllowCreate = privileges.AllowCreate,
                AllowMove = privileges.AllowMove,
                AllowDelete = privileges.AllowDelete,
                AllowRename = privileges.AllowRename,
                AllowCopy = privileges.AllowCopy,
                AllowDownload = privileges.AllowDownload
            };
            this.settingsToolbar = new FileManagerSettingsToolbar(null)
            {
                ShowPath = true,
                ShowFilterBox = true
            };



            this.settingsFolders = new FileManagerSettingsFolders(null)
            {
                Visible = true,
                EnableCallBacks = false,
                ShowFolderIcons = true,
                ShowLockedFolderIcons = true
            };
            this.settingsUpload = new MVCxFileManagerSettingsUpload();
            this.settingsUpload.Enabled = privileges.AllowUpload;
            this.settingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
        }

        private FileManagerPrivilege GetPrivilege(UserProfileSessionData sessionData)
        {
            ///dummy next should get from profile provider
            var privilege = new FileManagerPrivilege()
            {
                AllowCopy = true,
                AllowCreate = true,
                AllowDelete = true,
                AllowDownload = false,
                AllowMove = true,
                AllowRename = true,
                AllowUpload = true
            };
            return privilege;
        }



        [Display(Name = "Settings Editing")]
        public FileManagerSettingsEditing SettingsEditing { get { return settingsEditing; } }
        [Display(Name = "Settings Toolbar")]
        public FileManagerSettingsToolbar SettingsToolbar { get { return settingsToolbar; } }
        [Display(Name = "Settings Folders")]
        public FileManagerSettingsFolders SettingsFolders { get { return settingsFolders; } }
        [Display(Name = "Settings Upload")]
        public MVCxFileManagerSettingsUpload SettingsUpload { get { return settingsUpload; } }

        public class FileManagerPrivilege
        {
            public bool AllowCreate { get; set; }
            public bool AllowMove { get; set; }
            public bool AllowDelete { get; set; }
            public bool AllowRename { get; set; }
            public bool AllowCopy { get; set; }
            public bool AllowDownload { get; set; }
            public bool AllowUpload { get; set; }
        }
    }
    #endregion

}