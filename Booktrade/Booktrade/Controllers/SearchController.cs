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
                result = result.Where(x => x.Title.ToLower().Contains(phrase.ToLower()) || x.Author.ToLower().Contains(phrase.ToLower()) || (x.Publisher!=null ? x.Publisher.ToLower().Contains(phrase.ToLower()) : false)).ToList();
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
            List<Book> result = context.Books.Where(x => x.isChanged != true && x.isSold != true).ToList();
            SearchPartialModel m = new SearchPartialModel();
            if(model.Phrase!=null && model.Phrase != "")
            {
                result = result.Where(x => x.Title.ToLower().Contains(model.Phrase.ToLower()) || x.Author.ToLower().Contains(model.Phrase.ToLower())).ToList();
            }
            if(model.City!=null && model.City != "")
            {
                result = result.Where(x => x.Seller.City.ToLower().Contains(model.City.ToLower())).ToList();
            }
            if (model.Province != "Wszystkie")
            {
                result = result.Where(x => x.Seller.Province.ToLower().Contains(model.Province.ToLower())).ToList();
            }
            if (!(model.ForExchange == true && model.ForSell==true))
            {
                if(model.ForExchange==true && model.ForSell == false)
                {
                    result = result.Where(x => x.Changeable == true).ToList();
                }
                if(model.ForExchange==false && model.ForSell == true)
                {
                    result = result.Where(x => x.Price != 0).ToList();
                }
                if (model.ForExchange == false && model.ForSell == false)
                {
                    result = result.Where(x => x.Price == 0 && x.Changeable==false).ToList();
                }
            }
            if (model.ForSell == true)
            {
                if(model.PriceFrom!=0 && model.PriceTo != 0)
                {
                    result = result.Where(x => x.Price>=model.PriceFrom && x.Price<=model.PriceTo).ToList();
                }
            }
            if (model.Publisher!=null && model.Publisher != "")
            {
                result = result.Where(x => x.Publisher != null ? x.Publisher.ToLower().Contains(model.Publisher) : false).ToList();
            }
            if (model.PublicationYear != 0)
            {
                result = result.Where(x => x.PublicationDate != null ? x.PublicationDate.Value.Year==model.PublicationYear : false).ToList();
            }
            if (model.Category != null)
            {
                result = result.Where(x=>x.Genre==model.Category).ToList();
            }
            if (model.SortBy != "Any")
            {
                if (model.SortBy == "PriceAsc")
                {
                    result = result.OrderBy(o => o.Price).ToList();
                }
                if (model.SortBy == "PriceDesc")
                {
                    result = result.OrderByDescending(o => o.Price).ToList();
                }
                if (model.SortBy == "DateNew")
                {
                    result = result.OrderByDescending(o => o.AddDate).ToList();
                }
                if (model.SortBy == "DateOld")
                {
                    result = result.OrderBy(o => o.AddDate).ToList();
                }
            }
            m.MaxPages = (int)Math.Ceiling((double)result.Count / (double)model.ResultsForPage);
            m.Books = result.Skip((model.CurrentPage-1) * model.ResultsForPage).Take(model.ResultsForPage).ToList();
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Information", "Info", new { text = "Error" });
            }
            return PartialView("_SearchedBooks",m);
        }
    }
}