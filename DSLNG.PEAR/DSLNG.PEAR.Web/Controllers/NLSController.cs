using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class NLSController : Controller
    {
        //
        // GET: /NLS/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /NLS/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /NLS/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NLS/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NLS/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /NLS/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NLS/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /NLS/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
