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
            List<FileSystemItem> files = (List<FileSystemItem>)HttpContext.Current.Session["ProcessBlueprintItems"];
            if (files == null)
            {
                files = service.Gets(new GetProcessBlueprintsRequest { Take = 0, Skip = 0 }).ProcessBlueprints.ToList().MapTo<FileSystemItem>();
                HttpContext.Current.Session["ProcessBlueprintItems"] = files;
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
            return base.GetFiles(folder);
        }

        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            return base.GetFolders(parentFolder);
        }

        public override bool Exists(FileManagerFile file)
        {
            return base.Exists(file);
        }

        public override bool Exists(FileManagerFolder folder)
        {
            return base.Exists(folder);
        }
        public override Stream ReadFile(FileManagerFile file)
        {
            return base.ReadFile(file);
        }

        public override DateTime GetLastWriteTime(FileManagerFile file)
        {
            return base.GetLastWriteTime(file);
        }

        public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            base.CopyFile(file, newParentFolder);
        }

        public override long GetLength(FileManagerFile file)
        {
            return base.GetLength(file);
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