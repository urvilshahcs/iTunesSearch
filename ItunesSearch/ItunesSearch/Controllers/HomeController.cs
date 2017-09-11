using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItunesSearch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Taking Top 10 Search terms with maximum counts
            ViewBag.SearchCounts = new DataAccess.ItunesSearchDBEntities().SearchCounts.OrderByDescending(a => a.Count).Take(10).ToList();
            return View();
        }

        [HttpGet]
        public ActionResult Search(string Term)
        {
            try
            {
                var result = SearchAPI.ItunesAPI.Search(Term);

                ViewBag.value = Term;
                return View(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult CountAndGO(string URL, string SearchTerm)
        {
            DataAccess.ItunesSearchDBEntities db = new DataAccess.ItunesSearchDBEntities();

            //Finding the term in database.
            var _term = db.SearchCounts.Where(a => a.Term == SearchTerm.ToLower()).FirstOrDefault();
            if (_term != null)
            {
                //If term is present Count is added
                _term.Count++;
                db.Entry(_term).State = System.Data.EntityState.Modified;
            }
            else
            {
                //Term is saved in database
                db.SearchCounts.Add(new DataAccess.SearchCount() { Term = SearchTerm.ToLower(), Count = 1 });
            }
            db.SaveChanges();
            if (URL == null || URL == "")
               return RedirectToAction("NoURL");

            return Redirect(URL);
        }

        public ActionResult NoURL()
        {
            return View();
        }
    }
}