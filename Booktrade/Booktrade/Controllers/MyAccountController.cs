using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Booktrade.Models;
using System.Threading.Tasks;

namespace Booktrade.Controllers
{
    public class MyAccountController : Controller
    {
        // GET: MyAccount
        [HttpGet]
        public ActionResult MyBooks()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<ActionResult> MyAccount(RegisterModel model)
        //{

        //}

        [HttpGet]
        public ActionResult AddBooks()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddBooks(Book model)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var book = new Book
            {
                Author = model.Author,
                Title = model.Title,
                Genre = model.Genre,
                Description = model.Description,
                AddDate = model.AddDate,
                Buyer = model.Buyer,
                Price = model.Price,
                Publisher = model.Publisher,
                Seller = model.Seller,
                PublicationDate = model.PublicationDate
            };

            using (var context = new AppDbContext())
            {
                context.Books.Add(book);
                context.SaveChanges();
            }
            return View();


        }
    }
}