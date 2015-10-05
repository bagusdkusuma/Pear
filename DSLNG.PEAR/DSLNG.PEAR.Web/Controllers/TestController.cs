using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;

namespace DSLNG.PEAR.Web.Controllers
{
    public class TestController : BaseController
    {
        private readonly IDataContext _dataContext;

        public TestController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public ActionResult Index()
        {
            var username = _dataContext.Users.First(x => x.Id == 1).Username;
            AddKpi();
            return Content(username);
        }

        private void AddKpi()
        {
            var list = new Collection<KpiRelationModel>();
            var plantAvailability = _dataContext.Kpis.First(x => x.Id == 3);
            var item1 = new KpiRelationModel();
            item1.Kpi = plantAvailability;
            item1.Method = "Quantitative";
            list.Add(item1);

            var plantReliability = new Kpi
            {
                Name = "Plant Reliability",
                Measurement = _dataContext.Measurements.First(x => x.Id == 1),
                Pillar = _dataContext.Pillars.First(x => x.Id == 2),
                Order = 2,
                RelationModels = list
            };

            _dataContext.Kpis.Add(plantReliability);
            //_dataContext.Entry(plantReliability).State = System.Data.Entity.EntityState.Detached;
            _dataContext.SaveChanges();
        }
	}
}