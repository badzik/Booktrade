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
            if (text == "MessageSendFail")
            {
                temp = "Wystąpił problem podczas wysyłania wiadomości";
            }
            if (text == "Error")
            {
                temp = "Wystąpił błąd";
            }
            if(text== "isSold")
            {
                temp = "Książka została już kupiona";
            }
            if(text== "bought")
            {
                temp = "Kupno książki przebiegło pomyślnie";    
            }
            if(text=="RegisterConfirm")
            {
                temp = "Na podany adres e-mail została wysłana wiadomość aktywacyjna.";
            }
            if(text=="AccountActivated")
            {
                temp = "Konto zostało aktywowane. Można teraz się zalogować.";
            }
            return View((object)temp);
        }
    }
}