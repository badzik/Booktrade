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


        public MyAccountController() : this(Startup.UserManagerFactory.Invoke())
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
            var context = new AppDbContext();
            ICollection<Book> model = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).SellingBooks;
            return View(model);
        }
        //[HttpPost]
        //public async Task<ActionResult> MyAccount(RegisterModel model)
        //{

        //}

        [HttpGet]
        public ActionResult AddBook()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddBook(SellBookModel model, bool deliveryBool1, bool deliveryBool2, bool deliveryBool3, bool deliveryBool4)
        {
            List<bool> CheckList = new List<bool> { deliveryBool1, deliveryBool2, deliveryBool3, deliveryBool4 };


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
                foreach (string error in errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View();
            }
            var context = new AppDbContext();


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
                Changeable = model.Changeable,
                PublicationDate = model.PublicationDate,
                SellerId = System.Web.HttpContext.Current.User.Identity.GetUserId(),
                Seller = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),

            };
            context.Books.Add(book);
            context.SaveChanges();
            bool checkDelivery = false;
            if (deliveryBool1 == true)
            {
                checkDelivery = true;
                var delivery = new Delivery
                {
                    Name = "Odbiór osobisty",
                    Price = model.DeliveryPrice[0],
                    DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                    DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                };
                context.DeliveryOptions.Add(delivery);
                context.SaveChanges();
            }
            if (deliveryBool2 == true)
            {
                checkDelivery = true;
                var delivery = new Delivery
                {
                    Name = "Przesyłka pocztowa - priorytetowa",
                    Price = model.DeliveryPrice[1],
                    DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                    DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                };
                context.DeliveryOptions.Add(delivery);
                context.SaveChanges();
            }


            if (deliveryBool3 == true)
            {
                checkDelivery = true;
                var delivery = new Delivery
                {
                    Name = "Przesyłka pocztowa - ekonomiczna",
                    Price = model.DeliveryPrice[2],
                    DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                    DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                };
                context.DeliveryOptions.Add(delivery);
                context.SaveChanges();
            }
            if (deliveryBool4 == true)
            {
                checkDelivery = true;
                var delivery = new Delivery
                {
                    Name = "Przesyłka kurierska",
                    Price = model.DeliveryPrice[3],
                    DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                    DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                };
                context.DeliveryOptions.Add(delivery);
                context.SaveChanges();
            }


            if (!checkDelivery)
            {
                ModelState.AddModelError("", "Proszę wybrać opcję dostawy");
                return View();
            }

            byte[] uploadedFile = null;
            BookImage bookImage = null;
            if (Request.Files != null)
            {
                foreach (string image in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[image] as HttpPostedFileBase;
                    uploadedFile = new byte[hpf.InputStream.Length];
                    hpf.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    bookImage = new BookImage { Image = uploadedFile, BookImgId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId, BookImg = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault() };
                    context.BookImages.Add(bookImage);
                    context.SaveChanges();
                }




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

            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public ActionResult EditBook(int bookId)
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
            var editBook = new EditBookModel
            {
                Author = book.Author,
                Title = book.Title,
                Genre = book.Genre,
                Description = book.Description,
                //BookImages = ;
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

            return View(editBook);
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

        [HttpGet]
        public ActionResult Settings()
        {
            var context = new AppDbContext();
            var errMsg = TempData["ErrorMessage"] as string;
            AppUser user = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            SettingsViewModel settings = new SettingsViewModel()
            {
                SettingsModel = new UserSettingsModel()
                {
                    Address = user.Address,
                    BankAccount = user.BankNumber,
                    City = user.City,
                    Name = user.Name,
                    PostalCode = user.PostalCode,
                    Surname = user.Surname,
                    Province = user.Province
                }

            };
            if (errMsg != null)
            {
                ModelState.AddModelError("", errMsg);
            }
            return View(settings);
        }

        [HttpPost]
        public ActionResult SettingsChange(SettingsViewModel model)
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
                foreach (string error in errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View("Settings");
            }

            var context = new AppDbContext();
            AppUser user = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            user.Address = model.SettingsModel.Address;
            user.BankNumber = model.SettingsModel.BankAccount;
            user.City = model.SettingsModel.City;
            user.Name = model.SettingsModel.Name;
            user.Surname = model.SettingsModel.Surname;
            user.PostalCode = model.SettingsModel.PostalCode;
            user.Province = model.SettingsModel.Province;
            context.SaveChanges();

            return View("Settings");
        }

        [HttpPost]
        public ActionResult PasswordChange(SettingsViewModel model)
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
                foreach (string error in errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View("Settings");
            }
            if (model.NewPassword != model.NewPasswordRepeat)
            {
                TempData["ErrorMessage"] = "Podane hasła nie są takie same.";
                return RedirectToAction("Settings", "MyAccount", new { model = model });
            }
            IdentityResult result = userManager.ChangePassword(System.Web.HttpContext.Current.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Information", "Info", new { text = "PasswordChanged" });
            }
            else
            {
                TempData["ErrorMessage"] = "Nie udało się zmienić hasła.";
                return RedirectToAction("Settings", "MyAccount", new { model = model });
            }
        }
    }
}