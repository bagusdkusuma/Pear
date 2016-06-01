using DevExpress.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.File
{
    public class PdfPageViewModel
    {
        PdfDocumentProcessor _documentProcessor;
        public PdfPageViewModel(PdfDocumentProcessor documentProcessor)
        {
            this._documentProcessor = documentProcessor;
        }
        public PdfDocumentProcessor DocumentProcessor
        {
            get { return _documentProcessor; }
        }
        public int PageNumber { get; set; }
        public byte[] GetPageImageBytes()
        {
            using (Bitmap bitmap = DocumentProcessor.CreateBitmap(PageNumber, 900))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}