using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerTransactionController : BaseController
    {
        // GET: DerTransaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Input()
        {
            return View();
        }

        public ActionResult GetGlobalStockMarket()
        {            
            return PartialView("LayoutType/_GLobalStockMarket");
        }

    }
}