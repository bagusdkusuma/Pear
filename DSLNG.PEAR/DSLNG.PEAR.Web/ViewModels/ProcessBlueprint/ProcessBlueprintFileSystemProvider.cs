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

namespace DSLNG.PEAR.Web.ViewModels.ProcessBlueprint
{
    public static class ProcessBlueprintDataProvider
    {
        public static IProcessBlueprintService service { get { return ObjectFactory.Container.GetInstance<IProcessBlueprintService>(); } }
        public static List<FileSystemItem> GetAll()
        {
            List<FileSystemItem> files = (List<FileSystemItem>)HttpContext.Current.Session["FileSystemItem"];
            if (files == null)
            {
                files = service.All().ProcessBlueprints.ToList().MapTo<FileSystemItem>();
                HttpContext.Current.Session["FileSystemItem"] = files;
            }
            return files;
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
    }
    public class ProcessBlueprintFileSystemProvider : FileSystemProviderBase
    {
        const int RootItemId = 1;
        string rootFolderDisplayName;
        Dictionary<int, FileSystemItem> folderCache;
        public ProcessBlueprintFileSystemProvider(string rootFolder):base(rootFolder)
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

        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            FileSystemItem folderItem = FindFolderItem(folder);
            return from item in ProcessBlueprintDataProvider.GetAll()
                   where !item.IsFolder && item.ParentId == folderItem.FileId
                   select new FileManagerFile(this, folder, item.Name);
        }

        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            FileSystemItem folderItem = FindFolderItem(parentFolder);
            return (from item in FolderCache.Values
                    where item.IsFolder && folderItem.ParentId == item.FileId
                    select new FileManagerFolder(this, parentFolder, item.Name));
        }

        private FileSystemItem FindFolderItem(FileManagerFolder parentFolder)
        {
            return (from item in FolderCache.Values
                    where item.IsFolder && GetRelativeName(item) == parentFolder.RelativeName
                    select item).FirstOrDefault();
        }

        protected string GetRelativeName(FileSystemItem item)
        {
            if (item.FileId == RootItemId) return string.Empty;
            if (item.ParentId == RootItemId) return item.Name;
            if(!FolderCache.ContainsKey((int)item.ParentId)) return null;
            string name = GetRelativeName(FolderCache[(int)item.ParentId]);
            return name == null ? null : Path.Combine(name,item.Name);
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
            base.CopyFile(file, newParentFolder);
        }

        public override long GetLength(FileManagerFile file)
        {
            var fileItem = FindFileItem(file);
            return fileItem.Data.Length;
        }

        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            base.CreateFolder(parent, name);
        }

        public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            base.CopyFolder(folder, newParentFolder);
        }

        public override void DeleteFolder(FileManagerFolder folder)
        {
            base.DeleteFolder(folder);
        }

        public override void DeleteFile(FileManagerFile file)
        {
            base.DeleteFile(file);
        }
        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            base.MoveFile(file, newParentFolder);
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            base.MoveFolder(folder, newParentFolder);
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