using Booktrade.Models;
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
        public ActionResult SearchWindow(string category, string phrase)
        {
            var context = new AppDbContext();
            SearchModel model = new SearchModel();
            ICollection<Book> result = context.Books.Where(x => x.isChanged != true && x.isSold != true).ToList();
            if (category != null && category!="")
            {
                model.Category = category;
                result = result.Where(x => x.Genre == model.Category).ToList();
            }
            if (phrase != null && phrase!="")
            {
                model.Phrase = phrase;
                result = result.Where(x => x.Title.ToLower().Contains(phrase.ToLower()) || x.Author.ToLower().Contains(phrase.ToLower()) || x.Publisher.ToLower().Contains(phrase.ToLower())).ToList();
            }
            model.Results = result;
            model.ForSell = true;
            model.ForExchange = true;
            model.CurrentPage = 1;
            model.SortBy = "Any";
            model.NumberOfPages = (int)Math.Ceiling((double)result.Count / (double)10);
            model.ResultsForPage = 10;
            model.PublicationYear = 0;
            model.PriceFrom = 0;
            model.PriceTo = 0;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SearchWindow(SearchModel model)
        {
            var context = new AppDbContext();
            ICollection<Book> result = context.Books.Where(x => x.isChanged != true && x.isSold != true).ToList();
            SearchPartialModel m = new SearchPartialModel();
            //TODO: adjust result and pages to received model
            m.MaxPages = 33;
            m.Books = result;
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Information", "Info", new { text = "Error" });
            }
            return PartialView("_SearchedBooks",m);
        }
    }
}