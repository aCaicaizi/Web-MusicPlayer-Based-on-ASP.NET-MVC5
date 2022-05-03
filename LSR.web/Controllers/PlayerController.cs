using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LSR.Controllers
{
    public class PlayerController : Controller
    {
        // GET: Player
        public ActionResult Main()
        {
            return View();
        }
    }
}