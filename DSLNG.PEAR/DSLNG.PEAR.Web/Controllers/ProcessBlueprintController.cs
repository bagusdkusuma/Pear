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
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Services.Requests.FileManagerRolePrivilege;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ProcessBlueprintController : BaseController
    {
        //
        // GET: /ProcessBlueprint/
        [AuthorizeUser(AccessLevel = "AllowView")]
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
                selectedFolder = selectedFolder.Substring(0, selectedFolder.IndexOf('|'));
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

        public ActionResult CustomToolbarAction(string viewType)
        {
            HttpContext.Session["aspxCustomToolbarAction"] = viewType == "Thumbnails" ? FileListView.Thumbnails : FileListView.Details;
            return PartialView("_ProcessBlueprintPartial", ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider);
        }
        public PartialViewResult PrivilegeViewPartialView(FileSystemItem model)
        {
            List<FileManagerRolePrivilegeViewModel> models = new List<FileManagerRolePrivilegeViewModel>();
            ///todo create matrix of file vs role vs privilege
            if (model != null && model.FileId > 0)
            {
                var data = ProcessBlueprintDataProvider.service.GetPrivileges(new Services.Requests.ProcessBlueprint.GetProcessBlueprintPrivilegeRequest { FileId = model.FileId });
                if (data.IsSuccess)
                {
                    models = data.FileManagerRolePrivileges.ToList().MapTo<FileManagerRolePrivilegeViewModel>();
                }
            }

            return PartialView("_PrivilegePartial", models);
        }

        [ValidateInput(false)]
        public ActionResult PrivilegeUpdate(MVCxGridViewBatchUpdateValues<FileManagerRolePrivilegeViewModel, int> updateValues)
        {
            int fileId = 0;
            List<FileManagerRolePrivilegeViewModel> models = new List<FileManagerRolePrivilegeViewModel>();
            //todo here is create connection to service update FileManagerPrivileges
            BatchUpdateFilePrivilegeRequest request = new BatchUpdateFilePrivilegeRequest();
            var datas = new List<UpdateFilePrivilegeRequest>();
            foreach (var item in updateValues.Update)
            {
                var privilege = item.MapTo<UpdateFilePrivilegeRequest>();
                datas.Add(privilege);
                fileId = item.FileId;
            }
            request.BatchUpdateFilePrivilege = datas.ToList();
            var response = ProcessBlueprintDataProvider.service.BatchUpdateFilePrivilege(request);
            var data = ProcessBlueprintDataProvider.service.GetPrivileges(new Services.Requests.ProcessBlueprint.GetProcessBlueprintPrivilegeRequest { FileId = fileId });
            if (data.IsSuccess)
            {
                models = data.FileManagerRolePrivileges.ToList().MapTo<FileManagerRolePrivilegeViewModel>();
            }
            return PartialView("_PrivilegePartial", models);
        }
        public ActionResult ProcessConfigPartial(string relativePath, bool? isFileSelected)
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
                if (isFileSelected.HasValue && isFileSelected.Value == true)
                {
                    var file = new FileManagerFile(provider, selecetedFile);
                    item = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider.GetFile(file);
                }
                else
                {
                    var folder = new FileManagerFolder(provider, selecetedFile);
                    item = ProcessBlueprintControllerProcessBlueprintSettings.ProcessBlueprintFileSystemProvider.GetFolder(folder);
                }

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
            ".jpg", ".jpeg", ".gif", ".rtf", ".txt", ".png", ".doc",".docx", ".pdf",".xls",".xlsx", ".vsd", ".mpp", ".ppt", ".pptx"
        };

        static UserProfileSessionData sessionData = (UserProfileSessionData)HttpContext.Current.Session["LoginUser"];

        static FileManagerSettingsPermissions settings = new FileManagerSettingsPermissions(null);
        public static FileManagerSettingsPermissions SettingsPermissions { get { return settings; } }
        public static void ApplyRules(FileManagerFolder folder)
        {
            int fileId = 0;
            //set for my own files
            //var myFolder = ProcessBlueprintDataProvider.GetAll().FindAll(x => x.CreatedBy == sessionData.UserId).ToList();
            var fs = ProcessBlueprintFileSystemProvider.GetFolder(folder);
            if(fs == null) return;
            fileId = fs.FileId;
            var myFolder = ProcessBlueprintDataProvider.service.GetPrivileges(new Services.Requests.ProcessBlueprint.GetProcessBlueprintPrivilegeRequest { FileId=fileId, RoleGroupId = sessionData.RoleId  });
            if (myFolder.TotalRecords > 0)
            {
                foreach (var item in myFolder.FileManagerRolePrivileges)
                {
                    var file = item.ProcessBlueprint.MapTo<FileSystemItem>();
                    if (string.IsNullOrEmpty(file.Name)) continue;
                    var folderItem = ProcessBlueprintFileSystemProvider.GetRelativeName(file);
                    FileManagerAccessRuleBase rule = null;
                    if (item.ProcessBlueprint.IsFolder)
                    {
                        rule = new FileManagerFolderAccessRule();
                    }
                    else
                    {
                        rule = new FileManagerFileAccessRule();
                    }
                    rule.Path = folderItem;
                    rule.Browse = item.AllowBrowse ? Rights.Allow : Rights.Deny;
                    rule.Edit = item.AllowRename == item.AllowCreate == item.AllowCopy == item.AllowDelete == item.AllowMove ? Rights.Allow : Rights.Deny;
                    settings.AccessRules.Add(rule);
                }
            }
            else
            {
                var folderItem = ProcessBlueprintFileSystemProvider.GetRelativeName(fs);
                FileManagerAccessRuleBase rule = null;
                    
                if (fs.IsFolder)
                {
                    rule = new FileManagerFolderAccessRule();
                }
                else
                {
                    rule = new FileManagerFileAccessRule();
                }
                rule.Path = folderItem;
                rule.Browse = Rights.Default;
                rule.Edit = Rights.Default;
                settings.AccessRules.Add(rule);
            }
            //foreach (var item in myFolder.FileManagerRolePrivileges)
            //{
            //    FileManagerAccessRuleBase rule = null;
            //    // get folderitem
            //    var folderItem = ProcessBlueprintFileSystemProvider.GetRelativeName(item);
            //    if (item.IsFolder)
            //    {
            //        rule = new FileManagerFolderAccessRule();
            //        //settings.AccessRules.Add(new FileManagerFolderAccessRule(folderItem) { Edit = Rights.Allow, Browse = Rights.Allow, Role = sessionData.RoleName });
            //        //settings.AccessRules.Add(new FileManagerFolderAccessRule(folderItem) { EditContents = Rights.Allow, Role = sessionData.RoleName });
            //    }
            //    else
            //    {
            //        rule = new FileManagerFileAccessRule();
            //        //settings.AccessRules.Add(new FileManagerFileAccessRule(folderItem) { Edit = Rights.Allow, Role = sessionData.RoleName });
            //    }
            //    rule.Path = folderItem;
            //    rule.Browse = Rights.Allow;
            //    rule.Edit = Rights.Allow;
            //    settings.AccessRules.Add(rule);
            //}
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
        List<string> rights = (List<string>)HttpContext.Current.Session["ProcessBlueprint"];
        public FileManagerFeaturesOption()
        {
            if (rights == null) rights = new List<string>();
            this.settingsEditing = new FileManagerSettingsEditing(null)
            {
                AllowCopy = rights.Contains("AllowUpdate"),
                AllowCreate = rights.Contains("AllowCreate"),
                AllowDelete = rights.Contains("AllowDelete"),
                AllowDownload = rights.Contains("AllowDownload"),
                AllowMove = rights.Contains("AllowUpdate"),
                AllowRename = rights.Contains("AllowUpdate")
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
            this.settingsUpload.Enabled = rights.Contains("AllowUpload");
            this.settingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
        }
        #region old code
        //UserProfileSessionData sessionData = (UserProfileSessionData)HttpContext.Current.Session["LoginUser"];


        //public FileManagerFeaturesOption(string folder)
        //{
        //    FileManagerPrivilege privileges = GetPrivilege(sessionData);

        //    this.settingsEditing = new FileManagerSettingsEditing(null)
        //    {
        //        AllowCreate = privileges.AllowCreate,
        //        AllowMove = privileges.AllowMove,
        //        AllowDelete = privileges.AllowDelete,
        //        AllowRename = privileges.AllowRename,
        //        AllowCopy = privileges.AllowCopy,
        //        AllowDownload = privileges.AllowDownload
        //    };
        //    this.settingsToolbar = new FileManagerSettingsToolbar(null)
        //    {
        //        ShowPath = true,
        //        ShowFilterBox = true,
        //        ShowCopyButton = privileges.AllowCopy,
        //        ShowCreateButton = privileges.AllowCreate,
        //        ShowDeleteButton = privileges.AllowDelete,
        //        ShowDownloadButton = privileges.AllowDownload,
        //        ShowMoveButton = privileges.AllowMove,
        //        ShowRenameButton = privileges.AllowRename
        //    };



        //    this.settingsContextMenu = new FileManagerSettingsContextMenu(null)
        //    {
        //        Enabled = true
        //    };

        //    this.settingsFolders = new FileManagerSettingsFolders(null)
        //    {
        //        Visible = true,
        //        EnableCallBacks = false,
        //        ShowFolderIcons = true,
        //        ShowLockedFolderIcons = true
        //    };
        //    this.settingsUpload = new MVCxFileManagerSettingsUpload();
        //    this.settingsUpload.Enabled = privileges.AllowUpload;
        //    this.settingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
        //}



        //private FileManagerPrivilege GetPrivilege(UserProfileSessionData sessionData)
        //{
        //    ///dummy next should get from profile provider
        //    var privilege = new FileManagerPrivilege()
        //    {
        //        AllowCopy = false,
        //        AllowCreate = false,
        //        AllowDelete = true,
        //        AllowDownload = false,
        //        AllowMove = true,
        //        AllowRename = true,
        //        AllowUpload = false
        //    };
        //    return privilege;
        //}

        #endregion old code

        [Display(Name = "Settings Editing")]
        public FileManagerSettingsEditing SettingsEditing { get { return settingsEditing; } }
        [Display(Name = "Settings Toolbar")]
        public FileManagerSettingsToolbar SettingsToolbar { get { return settingsToolbar; } }
        [Display(Name = "Settings Folders")]
        public FileManagerSettingsFolders SettingsFolders { get { return settingsFolders; } }
        [Display(Name = "Settings Upload")]
        public MVCxFileManagerSettingsUpload SettingsUpload { get { return settingsUpload; } }
        //public class FileManagerPrivilege
        //{
        //    public bool AllowCreate { get; set; }
        //    public bool AllowMove { get; set; }
        //    public bool AllowDelete { get; set; }
        //    public bool AllowRename { get; set; }
        //    public bool AllowCopy { get; set; }
        //    public bool AllowDownload { get; set; }
        //    public bool AllowUpload { get; set; }
        //}
    }
    #endregion

}