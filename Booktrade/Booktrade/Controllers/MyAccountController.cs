using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Booktrade.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Booktrade.Controllers
{
    public class MyAccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public MyAccountController()
        {

        }

        public MyAccountController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

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
                Price = model.Price,
                Publisher = model.Publisher,
                PublicationDate = model.PublicationDate,
                BookImage = model.BookImage

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