using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Booktrade.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Adding new book-example
            //AppDbContext UsersContext = new AppDbContext();
            //List<AppUser> users = UsersContext.Users.ToList();
            //Book book = new Book()
            //{
            //    Title = "Test",
            //    User = users[0]
            //};
            //UsersContext.Books.Add(book);
            //UsersContext.SaveChanges();

            return View();
        }
    }
}