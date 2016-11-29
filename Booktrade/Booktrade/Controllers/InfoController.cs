using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booktrade.Controllers
{
    public class InfoController : Controller
    {
        // GET: Info
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Information(string text)
        {
            string temp = "Brak informacji";
            if (text == "GoToEmailRecovery")
            {
                temp = "Instrukcje o zresetowaniu hasła zostały wysłane na E-Mail.";
            }
            if (text == "PasswordChanged")
            {
                temp = "Hasło zostało zmienione.";
            }
            if(text== "PasswordChangeFailed")
            {
                temp = "Hasło nie zostało zmienione.";
            }
            if (text == "AccessDenied")
            {
                temp = "Brak dostępu.";
            }
            return View((object)temp);
        }
    }
}