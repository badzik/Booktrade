using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
            //Adding new book- example with image
            //var webClient = new WebClient();
            //byte[] imageBytes = webClient.DownloadData("http://www.google.com/images/logos/ps_logo2.png");
            //AppDbContext UsersContext = new AppDbContext();
            //List<AppUser> users = UsersContext.Users.ToList();
            //Book book = new Book()
            //{
            //    Title = "Test2",
            //    AddDate = new DateTime(2016,11,11),
            //    Author = "Jan Kochanowski2",
            //    Changeable = true,
            //    Description="Opis książki długi",
            //    Genre="Akcja",
            //    isSold=false,
            //    isChanged=false,
            //    Seller=users[0],
            //    Price=12,
            //    PublicationDate=new DateTime(1990,3,13),
            //    BookImage=imageBytes,
            //    Publisher="Wydawca"
            //};
            //UsersContext.Books.Add(book);
            //UsersContext.SaveChanges();
            return View();
        }
    }
}