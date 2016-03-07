using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class ESCategoryViewModel
    {
        public ESCategoryViewModel()
        {
            ESCategories = new List<ESCategory>();
        }
        public IList<ESCategory> ESCategories { get; set; }
        public class ESCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
        }
    }

    public class SaveESCategoryViewModel
    {
        public SaveESCategoryViewModel()
        {
            this.IsActive = true;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public List<SelectListItem> Types { get; set; }
    }

    public class GetESCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public List<SelectListItem> Types { get; set; }
    }
}