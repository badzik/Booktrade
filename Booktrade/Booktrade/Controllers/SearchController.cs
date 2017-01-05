using Booktrade.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booktrade.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult SearchWindow(string category,string phrase)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SearchWindow(SearchModel model)
        {
            return View(); //PartialView
        }
    }
}