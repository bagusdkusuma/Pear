using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.MidtermStrategyPlanning
{
    public class AddMidtermPlanningViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int MidtermStageId { get; set; }
    }
}