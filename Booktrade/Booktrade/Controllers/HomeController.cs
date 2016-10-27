using System;
using System.Collections.Generic;
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
            var context = new ApplicationDbContext();
            var allusers = userManager.Users.ToList();
            return View();
        }
    }
}