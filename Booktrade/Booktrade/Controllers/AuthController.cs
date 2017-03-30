using Booktrade.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using Facebook;
using Newtonsoft.Json;
using System;
using System.Web.Security;

namespace Booktrade.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public AuthController()
            : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public AuthController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new LogInModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindAsync(model.Email, model.Password);

            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ApplicationCookie);

                GetAuthenticationManager().SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Błędny login lub hasło");
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Address = model.Address,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                Province = model.Province,
                PostalCode = model.PostalCode,
                BankNumber = "Nie podano",
                City = model.City

            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignIn(user);
                return RedirectToAction("index", "home");
            }

            string temp;
            Regex regex = new Regex(@"Name .* is already taken.");
            foreach (var error in result.Errors)
            {
                temp = error;
                if (error == "Passwords must be at least 6 characters.")
                {
                    temp = "Hasło musi zawierać przynajmniej 6 znaków.";
                }
                if (regex.Match(error).Success)
                {
                    temp = "Istnieje konto dla podanego adresu email.";
                }
                ModelState.AddModelError("", temp);
            }

            return View();
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }

        private async Task SignIn(AppUser user)
        {
            var identity = await userManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
            GetAuthenticationManager().SignIn(identity);
        }

        [HttpGet]
        public ActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PasswordRecovery(string email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = userManager.FindByEmail(email);
            if (user != null)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                code = HttpUtility.UrlEncode(code);
                string link = "localhost:41655\\auth\\passwordChangeRecovery?token=" + code + "&id=" + user.Id;
                var fromAddress = new MailAddress("adm1n_b00ktrade@outlook.com", "Booktrade");
                var toAddress = new MailAddress(user.Email, user.Name);
                const string fromPassword = "Website007!";
                const string subject = "Odzyskiwanie hasła - booktrade";
                string body = "Witaj " + user.Name + ",</br> Poprosiłeś o zresetowanie Twojego hasła na portalu booktrade ponieważ zapomniałeś obecnego. Jeżeli nie wysyłałeś prośby zignoruj tą wiadomość.</br></br>"
                    + "Aby zresetować hasło, odwiedź poniższą stronę:</br>" + "<a>" + link + "</a>";

                var smtp = new SmtpClient
                {
                    Host = "smtp-mail.outlook.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }

            }
            else
            {
                ModelState.AddModelError("", "Konto dla podanego adresu email nie istnieje");
                return View();
            }
            return RedirectToAction("Information", "Info", new { text = "GoToEmailRecovery" });
        }

        [HttpGet]
        public ActionResult PasswordChangeRecovery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordChangeRecovery(string password1, string password2)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            string token = Request.QueryString["token"];
            string id = Request.QueryString["id"];
            if (password1 != password2)
            {
                ModelState.AddModelError("", "Podane hasła nie są takie same");
                return View();
            }
            if (id != null && token != null)
            {
                IdentityResult result = userManager.ResetPassword(id, token, password1);
                if (result.Succeeded)
                {
                    return RedirectToAction("Information", "Info", new { text = "PasswordChanged" });
                }
                else
                {
                    return RedirectToAction("Information", "Info", new { text = "PasswordChangeFailed" });
                }
            }
            else
            {
                return RedirectToAction("Information", "Info", new { text = "AccessDenied" });
            }


        }

        private Uri RediredUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("facebookCallBack");
                return uriBuilder.Uri;
            }
        }
        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "264620673990463",
                client_secret = "416afd5fa57caf031978b354eb67c902",
                redirect_uri = RediredUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        public async Task<ActionResult> FacebookCallBack(string code, LogInModel model)
        {
            var fb = new FacebookClient();
            RegisterModel registerModel = null;
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "264620673990463",
                client_secret = "416afd5fa57caf031978b354eb67c902",
                redirect_uri = RediredUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,location,locale,timezone,verified,picture,age_range");
            string email = me.email;
            AppUser user = userManager.FindByEmail(email);
            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(
    user, DefaultAuthenticationTypes.ApplicationCookie);
                GetAuthenticationManager().SignIn(identity);
                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }
            else
            {
                registerModel = new RegisterModel
                {
                    Email = me.email,
                    Name = me.first_name,
                    Surname = me.last_name,
                    //City = me.location.name
                };
                //TempData["email"] = me.email;
                //TempData["first_name"] = me.first_name;
                //TempData["lastname"] = me.last_name;
                //TempData["picture"] = me.picture.data.url;
                return RedirectToAction("ExternalRegistration", "Auth", registerModel);
            }
            //FormsAuthentication.SetAuthCookie(email, false);
        }

        [HttpGet]
        public ActionResult ExternalRegistration(RegisterModel model)
        {
            return View(model);
        }
        [ActionName("Registration")]
        [HttpPost]
        public async Task<ActionResult> ExternalRegistrationPost(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Address = model.Address,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                Province = model.Province,
                PostalCode = model.PostalCode,
                BankNumber = "Nie podano",
                City = model.City
            };

            //random password
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");
            var result = await userManager.CreateAsync(user, guidString);

            if (result.Succeeded)
            {
                await SignIn(user);
                return RedirectToAction("index", "home");
            }

            string temp;
            Regex regex = new Regex(@"Name .* is already taken.");
            foreach (var error in result.Errors)
            {
                temp = error;
                if (error == "Passwords must be at least 6 characters.")
                {
                    temp = "Hasło musi zawierać przynajmniej 6 znaków.";
                }
                if (regex.Match(error).Success)
                {
                    temp = "Istnieje konto dla podanego adresu email.";
                }
                ModelState.AddModelError("", temp);
            }
            return View();
        }
    }
}