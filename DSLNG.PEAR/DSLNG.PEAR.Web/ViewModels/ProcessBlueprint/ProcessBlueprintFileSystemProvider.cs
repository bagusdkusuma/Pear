using DevExpress.Web;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;

namespace DSLNG.PEAR.Web.ViewModels.ProcessBlueprint
{
    public static class ProcessBlueprintDataProvider
    {
        static UserProfileSessionData sessionData = (UserProfileSessionData)HttpContext.Current.Session["LoginUser"];
        public static IProcessBlueprintService service { get { return ObjectFactory.Container.GetInstance<IProcessBlueprintService>(); } }
        public static List<FileSystemItem> GetAll()
        {
            //List<FileSystemItem> files = (List<FileSystemItem>)HttpContext.Current.Session["FileSystemItem"];
            //if (files == null)
            //{
            //    files = service.All().ProcessBlueprints.ToList().MapTo<FileSystemItem>();
            //    HttpContext.Current.Session["FileSystemItem"] = files;
            //}

            return service.All().ProcessBlueprints.ToList().MapTo<FileSystemItem>();
        }

        public static void Insert(FileSystemItem file)
        {
            var response = service.Save(new SaveProcessBlueprintRequest
            {
                IsFolder = file.IsFolder,
                ParentId = file.ParentId,
                Name = file.Name,
                Data = file.Data,
                LastWriteTime = file.LastWriteTime,
                UserId = sessionData.UserId
            });
            if (response.IsSuccess)
            {
                file.FileId = response.Id;
                GetAll().Add(file);
                AddDefaultPrivilege(response.Id);
            }
        }

        private static void AddDefaultPrivilege(int processBlueprintId)
        {
            var roleid = sessionData.RoleId;
            service.InsertOwnerPrivilege(new Services.Requests.FileManagerRolePrivilege.FilePrivilegeRequest { 
                ProcessBlueprint_Id = processBlueprintId,
                RoleGroup_Id = roleid,
                AllowBrowse = true,
                AllowCopy = true,
                AllowCreate = true,
                AllowDelete = true,
                AllowDownload = true,
                AllowMove = true,
                AllowRename = true,
                AllowUpload = true
            });
        }
        public static void Update(FileSystemItem file)
        {
            service.Save(new SaveProcessBlueprintRequest
            {
                Id = file.FileId,
                ParentId = file.ParentId,
                IsFolder = file.IsFolder,
                Name = file.Name,
                Data = file.Data,
                LastWriteTime = file.LastWriteTime,
                UserId = sessionData.UserId
            });
        }

        public static void Delete(FileSystemItem file)
        {
            if (file.IsFolder)
            {
                List<FileSystemItem> childFolders = GetAll().FindAll(item => item.IsFolder && item.ParentId == file.FileId);
                if (childFolders != null && childFolders.Count > 0)
                {
                    foreach (var childFolder in childFolders)
                    {
                        Delete(childFolder);
                    }
                }
                List<FileSystemItem> files = GetAll().FindAll(item => item.ParentId == file.FileId);
                if (files != null && files.Count > 0)
                {
                    foreach (var item in files)
                    {
                        Delete(item);
                    }
                }
            }
            if (service.Delete(file.FileId).IsSuccess)
            {
                GetAll().Remove(file);
            }
        }
    }
    public class FileSystemItem
    {
        public int FileId { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public bool IsFolder { get; set; }

        public byte[] Data { get; set; }

        public DateTime? LastWriteTime { get; set; }
        public int CreatedBy { get; set; }
    }
    public class ProcessBlueprintFileSystemProvider : FileSystemProviderBase
    {
        const int RootItemId = 1;
        string rootFolderDisplayName;
        Dictionary<int, FileSystemItem> folderCache;
        public ProcessBlueprintFileSystemProvider(string rootFolder)
            : base(rootFolder)
        {
            RefreshFolderCache();
        }

        public override string RootFolderDisplayName
        {
            get
            {
                return rootFolderDisplayName;
            }
        }
        public Dictionary<int, FileSystemItem> FolderCache { get { return folderCache; } }

        public override string GetDetailsCustomColumnDisplayText(FileManagerDetailsColumn column)
        {
            return base.GetDetailsCustomColumnDisplayText(column);
        }
        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            if (string.IsNullOrEmpty(folder.Name.ToString()))
            {
                return null;
            }
            FileSystemItem folderItem = FindFolderItem(folder);
            return from item in ProcessBlueprintDataProvider.GetAll()
                   where !item.IsFolder && item.ParentId == folderItem.FileId
                   select new FileManagerFile(this, folder, item.Name);
        }

        public FileSystemItem GetFile(FileManagerFile file)
        {
            return FindFileItem(file);
        }

        public FileSystemItem GetFolder(FileManagerFolder folder)
        {
            return FindFolderItem(folder);
        }
        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            FileSystemItem folderItem = FindFolderItem(parentFolder);
            var response = (from item in FolderCache.Values
                            where item.IsFolder && folderItem.FileId == item.ParentId
                            select new FileManagerFolder(this, parentFolder, item.Name));
            return response;
        }

        private FileSystemItem FindFolderItem(FileManagerFolder parentFolder)
        {
            var response = (from item in FolderCache.Values
                            where item.IsFolder && GetRelativeName(item) == parentFolder.RelativeName
                            select item).FirstOrDefault();
            return response;
        }

        public string GetRelativeName(FileSystemItem item)
        {
            //if (item.FileId == RootItemId) return string.Empty;
            //if (item.ParentId == RootItemId) return item.Name;
            //if (!FolderCache.ContainsKey((int)item.ParentId)) return null;
            //var parent = FolderCache[(int)item.ParentId];
            //string name = GetRelativeName(FolderCache[(int)item.ParentId]);
            //return name == null ? null : Path.Combine(name, item.Name);

            if (item.FileId == RootItemId) return string.Empty;
            if (item.ParentId == RootItemId) return item.Name;
            if (!FolderCache.ContainsKey((int)item.ParentId)) return null;
            //var parent = FolderCache[(int)item.ParentId];
            string name = GetRelativeName(FolderCache[(int)item.ParentId]);
            //if (parent.ParentId != -1 && parent.FileId > 1)
            //{
            //    name = Path.Combine(GetRelativeName(FolderCache[(int)parent.ParentId]), name);
            //}
            string result = name == null ? null : Path.Combine(name, item.Name);
            return result;
        }

        public override bool Exists(FileManagerFile file)
        {
            return FindFileItem(file) != null;
        }

        protected FileSystemItem FindFileItem(FileManagerFile file)
        {
            FileSystemItem fileItem = FindFolderItem(file.Folder);
            if (fileItem == null)
                return null;
            return ProcessBlueprintDataProvider.GetAll().FindAll(item => (int)item.ParentId == fileItem.FileId && !item.IsFolder && item.Name == file.Name).FirstOrDefault();
        }

        public override bool Exists(FileManagerFolder folder)
        {
            return FindFolderItem(folder) != null;
        }

        public override void UploadFile(FileManagerFolder folder, string fileName, Stream content)
        {
            ProcessBlueprintDataProvider.Insert(new FileSystemItem
            {
                IsFolder = false,
                LastWriteTime = DateTime.Now,
                Name = fileName,
                ParentId = FindFolderItem(folder).FileId,
                Data = ReadAllBytes(content)
            });
            //base.UploadFile(folder, fileName, content);
        }
        public override Stream ReadFile(FileManagerFile file)
        {
            return new MemoryStream(FindFileItem(file).Data.ToArray());
        }

        public override DateTime GetLastWriteTime(FileManagerFile file)
        {
            var fileItem = FindFileItem(file);
            return fileItem.LastWriteTime.GetValueOrDefault(DateTime.Now);
        }

        public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            var fileData = FindFileItem(file);
            if (fileData != null)
            {
                ProcessBlueprintDataProvider.Insert(new FileSystemItem
                {
                    IsFolder = false,
                    ParentId = FindFolderItem(newParentFolder).FileId,
                    Name = fileData.Name,
                    Data = fileData.Data,
                    LastWriteTime = DateTime.Now
                });
            }
            
            RefreshFolderCache();
        }

        public override void RenameFile(FileManagerFile file, string name)
        {
            ProcessBlueprintDataProvider.Update(new FileSystemItem
            {
                FileId = FindFileItem(file).FileId,
                IsFolder = false,
                Name = name,
                LastWriteTime = DateTime.Now
            });
            RefreshFolderCache();
        }

        public override long GetLength(FileManagerFile file)
        {
            var fileItem = FindFileItem(file);
            return fileItem.Data.Length;
        }

        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            ProcessBlueprintDataProvider.Update(new FileSystemItem
            {
                FileId = FindFolderItem(folder).FileId,
                IsFolder = true,
                Name = name,
                ParentId = FindFolderItem(folder).ParentId,
                LastWriteTime = DateTime.Now
            });
            RefreshFolderCache();
        }
        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            ProcessBlueprintDataProvider.Insert(new FileSystemItem
            {
                IsFolder = true,
                Name = name,
                ParentId = FindFolderItem(parent).FileId,
                LastWriteTime = DateTime.Now
            });
            RefreshFolderCache();
        }

        public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            ProcessBlueprintDataProvider.Insert(new FileSystemItem
            {
                IsFolder = true,
                Name = folder.Name,
                ParentId = FindFolderItem(newParentFolder).ParentId,
                LastWriteTime = DateTime.Now
            });
        }

        public override void DeleteFolder(FileManagerFolder folder)
        {
            ProcessBlueprintDataProvider.Delete(FindFolderItem(folder));
            RefreshFolderCache();
        }

        public override void DeleteFile(FileManagerFile file)
        {
            ProcessBlueprintDataProvider.Delete(FindFileItem(file));

        }
        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            var fileData = FindFileItem(file);
            if (fileData != null)
            {
                ProcessBlueprintDataProvider.Update(new FileSystemItem
                {
                    FileId = fileData.FileId,
                    ParentId = FindFolderItem(newParentFolder).ParentId,
                    Name = file.Name,
                    Data = fileData.Data,
                    LastWriteTime = DateTime.Now
                });
            }
            RefreshFolderCache();
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            var folderData = FindFolderItem(folder);
            ProcessBlueprintDataProvider.Update(new FileSystemItem
            {
                FileId = folderData.FileId,
                ParentId = FindFolderItem(newParentFolder).ParentId,
                Name = folderData.Name,
                LastWriteTime = DateTime.Now
            });
            RefreshFolderCache();
        }
        protected void RefreshFolderCache()
        {
            this.folderCache = ProcessBlueprintDataProvider.GetAll().FindAll(x => x.IsFolder).ToDictionary(x => x.FileId);
            this.rootFolderDisplayName = (from folderItem in FolderCache.Values where folderItem.FileId == RootItemId select folderItem.Name).First();
        }
        protected static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            int readCount;
            using (MemoryStream ms = new MemoryStream())
            {
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }
                return ms.ToArray();
            }
        }
    }
}