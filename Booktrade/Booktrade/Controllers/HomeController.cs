﻿using Booktrade.Models;
using Booktrade.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Book(int bookId)
        {
            var context = new AppDbContext();
            Book book = context.Books.Find(bookId);
            var errMsg = TempData["ErrorMessage"] as string;
            List<string> bookImages = new List<string>();
            foreach (BookImage b in book.ImagesForBook)
            {
                string imageSrc = "/Images/noFoto.png";

                var firstImage = b;
                string imageBase64 = Convert.ToBase64String(firstImage.Image);
                imageSrc = string.Format("data:image/gif;base64,{0}", imageBase64);
                Debug.WriteLine(imageSrc);
                bookImages.Add(imageSrc);
            }
            List<float> deliveryPrices = new List<float>();
            List<string> deliveryNames = new List<string>();
            foreach (var d in book.DeliveryforBook)
            {
                deliveryPrices.Add(d.Price);
                deliveryNames.Add(d.Name);
            }
            var bookView = new EditBookModel
            {
                Id = bookId,
                Author = book.Author,
                Title = book.Title,
                Genre = book.Genre,
                Description = book.Description,
                Price = book.Price,
                Publisher = book.Publisher,
                Changeable = book.Changeable,
                PublicationDate = book.PublicationDate,
                BookImages = bookImages,
                Seller = book.Seller,
                DeliveryName = deliveryNames,
                DeliveryPrice = deliveryPrices
            };


            if (errMsg != null)
            {
                ModelState.AddModelError("", errMsg);
            }

            return View(bookView);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Usr(string userId,int? first, bool? next)
        {
            bool n = false;
            if (next != null)
            {
                n = true;
            }
            Debug.WriteLine("userId w get" + userId);
            int l = 0;
            int f = 0;
            if (first == null)
            {
                l = 10;
                f = 0;
            }
            else {
                if (n)
                {
                    l = first.Value;
                    f = first.Value;
                }
                else
                {
                    l = first.Value;
                }
            }
            var context = new AppDbContext();
            AppUser appUser = context.Users.Find(userId);
            ICollection<Book> myAllBooks = context.Users.Find(userId).SellingBooks;
            ICollection<Book> myBooks2 = new Collection<Book>();
            ICollection<Book> myBooks = new Collection<Book>();
            foreach (Book b in myAllBooks.Take(l))
            {
                myBooks2.Add(b);
            }
            foreach (Book b in myBooks2.Skip(f / 2))
            {
                myBooks.Add(b);
            }

            UserViewModel uvm = new UserViewModel
            {
                Address = appUser.Address,
                City = appUser.City,
                Name = appUser.Name,
                PostalCode = appUser.PostalCode,
                Province = appUser.Province,
                ReceivedComments = appUser.ReceivedComments,
                Books = myBooks,
                HowManyInOnePage = l,
                AllBooks = myAllBooks,
                userId = userId              
            };
            return View(uvm);
        }



        [HttpPost]
        public ActionResult Usr(UserViewModel uvm,string userId)
        {
            Debug.WriteLine("Post");
            Debug.WriteLine(userId);
            return RedirectToAction("Usr", "Home", new { userId = uvm.userId, first = uvm.HowManyInOnePage });
        }
    }
}