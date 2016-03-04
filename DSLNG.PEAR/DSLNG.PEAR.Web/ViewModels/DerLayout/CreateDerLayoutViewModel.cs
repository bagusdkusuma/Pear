


using System.Collections.Generic;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class CreateDerLayoutViewModel
    {
        public CreateDerLayoutViewModel()
        {
            /*LayoutItems = new List<DerLayoutItemViewModel>();


            LayoutItems.Add(new DerLayoutItemViewModel
                {
                    Column = 1,
                    Row = 1
                });

            LayoutItems.Add(new DerLayoutItemViewModel
            {
                Column = 1,
                Row = 2
            });

            LayoutItems.Add(new DerLayoutItemViewModel
            {
                Column = 1,
                Row = 3
            });

            LayoutItems.Add(new DerLayoutItemViewModel
            {
                Column = 1,
                Row = 4
            });*/
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        /*public IList<DerLayoutItemViewModel> LayoutItems { get; set; }
        public IList<SelectListItem> Types { get; set; }*/
    }
}
