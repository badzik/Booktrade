using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Booktrade.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Diagnostics;
using System.IO;
using System.Web.Helpers;
using Booktrade.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;

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
        public ActionResult AddBooks(SellBookModel model)
        {

            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var modelStateVal in ViewData.ModelState.Values)
                {
                    foreach (var error in modelStateVal.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        errors.Add(errorMessage);
                    }
                }
                foreach(string error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                
                return View();
            }
            var context = new AppDbContext();


            byte[] uploadedFile = null;
            if (model.BookImage != null)
            {
                uploadedFile = new byte[model.BookImage.InputStream.Length];
                model.BookImage.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                //próba skalowania obrazu
                //Image image = Image.FromStream(new MemoryStream(uploadedFile));
                //Image newImage;
                //newImage = ScaleImage(image, 300, 150);
                //var ms = new MemoryStream();
                //newImage.Save(ms, ImageFormat.Gif);
                //ms.ToArray();
                //uploadedFile = ms.ToArray();
                //model.BookImage.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
            }
                if (model.Price == null)
            {
                model.Price = 0;
            }

            var book = new Book
            {
                Author = model.Author,
                Title = model.Title,
                Genre = model.Genre,
                Description = model.Description,
                AddDate = DateTime.Now,
                Price = model.Price,
                Publisher = model.Publisher,
                Changeable= model.Changeable,
                PublicationDate = model.PublicationDate,
                BookImage = uploadedFile,
                SellerId = System.Web.HttpContext.Current.User.Identity.GetUserId(),
                Seller = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),
        };
                context.Books.Add(book);
                context.SaveChanges();
           

            return RedirectToAction("index", "home");
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

    }
}