using DevExpress.Web;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.Files;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.File;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MIRUploadControlSettings
    {
        public static UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".pdf" },
            MaxFileSize = 4194304
        };
        public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                HttpContext.Current.Session[e.UploadedFile.FileName] = e.UploadedFile.FileBytes;
                e.CallbackData = e.UploadedFile.FileName;
            }
        }
    }
}
