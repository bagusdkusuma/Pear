using DevExpress.Web;
using System.IO;
using System.Web.UI;

namespace DSLNG.PEAR.Web.Extensions
{
    public class ExcelUploadHelper
    {
        private static string templatePath;
        private static string targetPath;
        private static string[] extensions = new string[] { ".xls", ".xlsx", ".csv", };
        private static long maxSize = 20971520;

        public static UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = extensions,
            MaxFileSize = maxSize,
        };

        public static void setPath(string template, string target){
            templatePath = template;
            targetPath = target;
        }

        public static void setValidationSettings(string[] allowedExtension, long maxFileSize) {
            extensions = allowedExtension;
            maxSize = maxFileSize;
        }

        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                var path = System.Web.HttpContext.Current.Request.MapPath(targetPath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string resultFilePath = System.Web.HttpContext.Current.Request.MapPath(targetPath+ e.UploadedFile.FileName);
                e.UploadedFile.SaveAs(resultFilePath, true);//Code Central Mode - Uncomment This Line
                IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                if (urlResolver != null)
                {
                    e.CallbackData = Path.GetFileName(resultFilePath);//urlResolver.ResolveClientUrl(resultFilePath);
                }
            }
        }
    }
}