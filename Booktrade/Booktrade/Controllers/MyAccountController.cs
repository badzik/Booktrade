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


        [HttpGet]
        public ActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBook(SellBookModel model, bool deliveryBool1, bool deliveryBool2, bool deliveryBool3, bool deliveryBool4, IEnumerable<HttpPostedFileBase> files)
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
            Debug.WriteLine(context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).BankNumber + model.Price + deliveryBool1);
            if (context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).BankNumber == "Nie podano" && model.Price != 0 && deliveryBool1==false)
            {
               
                ModelState.AddModelError("", "W ustawieniach użytkownika nie podano numeru konta potrzebnego do ewentualnej finalizacji tranzakcji. Aby wystawić książkę na sprzedaż należy uzupełnić te dane.");
                return View();
            }

            {

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
                isChanged = false,
                isSold = false,
                Price = model.Price,
                Publisher = model.Publisher,
                Changeable = model.Changeable,
                PublicationDate = model.PublicationDate,
                SellerId = System.Web.HttpContext.Current.User.Identity.GetUserId(),
                Seller = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),

            };
            bool bookIsAdded = false;
            bool checkDelivery = false;
            if (deliveryBool1 == true)
            {
                if (!bookIsAdded)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                    bookIsAdded = true;
                }
                bookIsAdded = true;
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
                if (!bookIsAdded)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                    bookIsAdded = true;
                }
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
                if (!bookIsAdded)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                    bookIsAdded = true;
                }
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
                if (!bookIsAdded)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                    bookIsAdded = true;
                }
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
                int i = 0;
                foreach (string image in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[image] as HttpPostedFileBase;
                    if (image != "files")
                    {
                        if (hpf.InputStream.Length > 0)
                        {
                            uploadedFile = new byte[hpf.InputStream.Length];
                            Debug.WriteLine(uploadedFile.Length);
                            hpf.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                            bookImage = new BookImage { Image = uploadedFile, BookImgId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId, BookImg = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault() };
                            context.BookImages.Add(bookImage);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        HttpPostedFileBase hpf2 = Request.Files.Get(i);
                        if (hpf2.InputStream.Length > 0)
                        {
                            uploadedFile = new byte[hpf2.InputStream.Length];
                            Debug.WriteLine(uploadedFile.Length);
                            hpf2.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                            bookImage = new BookImage { Image = uploadedFile, BookImgId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId, BookImg = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault() };
                            context.BookImages.Add(bookImage);
                            context.SaveChanges();
                        }
                    }
                    i++;
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
            LuceneSearchIndexer.UpdateBooksIndex();
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
            Dictionary<string, float> deliveryDict = new Dictionary<string, float>();
            foreach (var d in book.DeliveryforBook)
            {
                deliveryPrices.Add(d.Price);
                deliveryNames.Add(d.Name);
            }

            deliveryDict.Add("Odbiór osobisty", 0.0f);
            deliveryDict.Add("Przesyłka pocztowa - priorytetowa", 12.0f);
            deliveryDict.Add("Przesyłka pocztowa - ekonomiczna", 10.0f);
            deliveryDict.Add("Przesyłka kurierska", 15.0f);
            var editBook = new EditBookModel
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
                DeliveryPrice = deliveryPrices,
                DeliveryDict = deliveryDict,
            };


            if (errMsg != null)
            {
                ModelState.AddModelError("", errMsg);
            }

            return View(editBook);
        }

        [HttpPost]
        public ActionResult EditBook(EditBookModel model, bool deliveryBool1, bool deliveryBool2, bool deliveryBool3, bool deliveryBool4)
        {
            int counter = 0;

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
            var book = context.Books.Find(model.Id);
            if (model.Price == null)
            {
                model.Price = 0;
            }

            book.Author = model.Author;
            book.Title = model.Title;
            book.Genre = model.Genre;
            book.Description = model.Description;
            book.AddDate = DateTime.Now;
            book.Price = model.Price;
            book.Publisher = model.Publisher;
            book.Changeable = model.Changeable;
            book.PublicationDate = model.PublicationDate;
            book.SellerId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            book.Seller = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());



            bool bookIsAdded = false;
            bool checkDelivery = false;
            if (deliveryBool1 == true)
            {
                counter++;
                if (!bookIsAdded)
                {
                    context.SaveChanges();
                    bookIsAdded = true;
                }
                checkDelivery = true;

                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Odbiór osobisty");
                if (oneDelivery != null)
                {
                    oneDelivery.Price = model.DeliveryDict["Odbiór osobisty"];
                    oneDelivery.DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId;
                    oneDelivery.DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault();
                    context.SaveChanges();
                }
                else
                {
                    var del = new Delivery
                    {
                        Name = "Odbiór osobisty",
                        Price = model.DeliveryDict["Odbiór osobisty"],
                        DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                        DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                    };
                    context.DeliveryOptions.Add(del);
                    context.SaveChanges();
                }
            }
            else
            {
                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Odbiór osobisty");
                if (oneDelivery != null)
                {
                    context.DeliveryOptions.Remove(oneDelivery);
                }
            }
            if (deliveryBool2 == true)
            {
                counter++;
                if (!bookIsAdded)
                {
                    context.SaveChanges();
                    bookIsAdded = true;
                }
                checkDelivery = true;
                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka pocztowa - priorytetowa");
                if (oneDelivery != null)
                {
                    oneDelivery.Price = model.DeliveryDict["Przesyłka pocztowa - priorytetowa"];
                    oneDelivery.DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId;
                    oneDelivery.DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault();
                    context.SaveChanges();
                }
                else
                {
                    var del = new Delivery
                    {
                        Name = "Przesyłka pocztowa - priorytetowa",
                        Price = model.DeliveryDict["Przesyłka pocztowa - priorytetowa"],
                        DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                        DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                    };
                    context.DeliveryOptions.Add(del);
                    context.SaveChanges();
                }

            }
            else
            {
                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka pocztowa - priorytetowa");
                if (oneDelivery != null)
                {
                    context.DeliveryOptions.Remove(oneDelivery);
                }
            }
            if (deliveryBool3 == true)
            {
                counter++;
                if (!bookIsAdded)
                {
                    context.SaveChanges();
                    bookIsAdded = true;
                }
                checkDelivery = true;

                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka pocztowa - ekonomiczna");
                if (oneDelivery != null)
                {
                    oneDelivery.Price = model.DeliveryDict["Przesyłka pocztowa - ekonomiczna"];
                    oneDelivery.DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId;
                    oneDelivery.DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault();
                    context.SaveChanges();
                }
                else
                {
                    var del = new Delivery
                    {
                        Name = "Przesyłka pocztowa - ekonomiczna",
                        Price = model.DeliveryDict["Przesyłka pocztowa - ekonomiczna"],
                        DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                        DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                    };
                    context.DeliveryOptions.Add(del);
                    context.SaveChanges();
                }
            }
            else
            {
                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka pocztowa - ekonomiczna");
                if (oneDelivery != null)
                {
                    context.DeliveryOptions.Remove(oneDelivery);
                }
            }

            if (deliveryBool4 == true)
            {
                counter++;
                if (!bookIsAdded)
                {
                    context.Books.Attach(book);
                    context.SaveChanges();
                    bookIsAdded = true;
                }
                checkDelivery = true;

                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                Debug.WriteLine(delivery.Count() + " " + counter);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka kurierska");
                if (oneDelivery != null)
                {
                    oneDelivery.Price = model.DeliveryDict["Przesyłka kurierska"];
                    oneDelivery.DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId;
                    oneDelivery.DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault();
                    context.SaveChanges();
                }
                else
                {
                    var del = new Delivery
                    {
                        Name = "Przesyłka kurierska",
                        Price = model.DeliveryDict["Przesyłka kurierska"],
                        DeliveryPriceId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId,
                        DeliveryPrices = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault()
                    };
                    context.DeliveryOptions.Add(del);
                    context.SaveChanges();
                }
            }
            else
            {
                var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == model.Id);
                var oneDelivery = delivery.SingleOrDefault(m => m.Name == "Przesyłka kurierska");
                if (oneDelivery != null)
                {
                    context.DeliveryOptions.Remove(oneDelivery);
                }
            }
            if (!checkDelivery)
            {
                ModelState.AddModelError("", "Proszę wybrać opcję dostawy");
                return View();
            }

            byte[] uploadedFile = null;
            if (Request.Files != null && Request.Files.Count > 0)
            {
                int i = 0;
                foreach (string image in Request.Files)
                {
                    HttpPostedFileBase hpf2 = Request.Files.Get(i); 
                    if (hpf2.InputStream.Length > 0)
                    {
                        Debug.WriteLine(image);
                        uploadedFile = new byte[hpf2.InputStream.Length];
                        hpf2.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        var img = context.BookImages.Where(m => m.BookImgId == model.Id);


                        BookImage[] imgTab = img.ToArray();
                        if (imgTab.Count() == 0)
                        {
                            var newImage = new BookImage
                            {
                                Image = uploadedFile,
                                BookImg = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault(),
                                BookImgId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId
                            };
                            context.BookImages.Add(newImage);
                            context.SaveChanges();
                        }
                        else
                        {
                            string nameImputImage = "BookImage" + i;
                            Debug.WriteLine("Przy edytowaniu książki: nameImputImage = ", nameImputImage, "image = ", image);
                            if (nameImputImage == image)
                            {
                                int imageId = imgTab[i].ImageId;
                                Debug.WriteLine("imageID = ", imageId);
                                img.Single(m => m.ImageId == imageId).Image = uploadedFile;
                                img.Single(m => m.ImageId == imageId).BookImgId = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault().BookId;
                                img.Single(m => m.ImageId == imageId).BookImg = context.Books.OrderByDescending(o => o.BookId).FirstOrDefault();
                                context.SaveChanges();
                            }
                        }

                    }
                }
                i++;
            }
            LuceneSearchIndexer.UpdateBooksIndex();
            return RedirectToAction("index", "home");
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

        public ActionResult DeleteBook(int bookId)
        {
            var context = new AppDbContext();
            var book = context.Books.SingleOrDefault(m => m.BookId == bookId);
            var images = context.BookImages.Where(m => m.BookImgId == bookId);
            var delivery = context.DeliveryOptions.Where(m => m.DeliveryPriceId == bookId);
            var transaction =  context.Transactions.SingleOrDefault(m => m.BookId == bookId);

            var exchangeMessages = context.ExchangeMessages.Where(m => m.BookId == bookId);

            context.SaveChanges();
            foreach (var d in delivery)
            {
                context.DeliveryOptions.Remove(d);
            }
            context.SaveChanges();
            foreach (var img in images)
            {
                context.BookImages.Remove(img);
            }
            context.SaveChanges();
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                context.SaveChanges();
            }
            foreach (var eM in exchangeMessages)
            {
                context.ExchangeMessages.Remove(eM);
            }
            context.SaveChanges();
            context.Books.Remove(book);
            context.SaveChanges();
            LuceneSearchIndexer.UpdateBooksIndex();

            return RedirectToAction("myBooks", "MyAccount");
        }
    }

 
}