using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.File
{
    public class FileRepositoryGridViewModel
    {
        public FileRepositoryGridViewModel()
        {
            FileRepositories = new List<FileRepository>();
        }
        public int Year { get; set; }
        public IList<FileRepository> FileRepositories { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }

        public class FileRepository
        {
            public int Id { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public string MonthName
            {
                get
                {
                    return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);// Month.ToString();
                }
            }

            public int ParentId { get; set; }
            public string Name { get; set; }
            public string Summary { get; set; }
            public string Filename { get; set; }
            public byte[] Data { get; set; }

            public DateTime? LastWriteTime { get; set; }
        }
    }
}
