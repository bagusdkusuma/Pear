using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.InputData
{
    public class CreateInputDataViewModel
    {
        public CreateInputDataViewModel()
        {
            Accountabilities = new List<SelectListItem>();
            PeriodeTypes = new List<SelectListItem>();
            GroupInputs = new List<GroupInput>();
            Kpis = new List<Kpi>();
        }

        public string Name { get; set; }
        public string PeriodeType { get; set; }
        [Display(Name="Accountability")]
        public int Accountability { get; set; }
        public int  Order { get; set; }
        public IList<SelectListItem> Accountabilities { get; set; }        
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public IList<GroupInput> GroupInputs { get; set; }
        public IList<Kpi> Kpis { get; set; }


        public class GroupInput
        {
            public GroupInput()
            {
                Kpis = new List<Kpi>();
            }

            public string Name { get; set; }
            public IList<Kpi> Kpis { get; set; }
            public int Order { get; set; }
        }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


    }
}