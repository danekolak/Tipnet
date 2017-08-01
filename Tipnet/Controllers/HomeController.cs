using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tipnet.Controllers
{
    public class HomeController : ControllerBase
    {

        // GET: Home
        public ActionResult Index(string ReturnUrl)
        {
            if(ReturnUrl != null)
                return RedirectToAction("LogOff","Account");
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}