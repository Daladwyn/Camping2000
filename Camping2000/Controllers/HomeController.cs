using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Camping2000.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpaceForTent()
        {
            ViewBag.Message = "Renting space for tent.";

            return PartialView("_SpaceForTent");
        }

        public ActionResult SpaceForCaravan()
        {
            ViewBag.Message = "Renting space for caravan.";

            return PartialView("_SpaceForCaravan");
        }
        public ActionResult ConfirmSpaceForTent()
        {
            return PartialView("_ConfirmSpaceForTent");
        }
        public ActionResult ConfirmSpaceForCaravan()
        {
            return PartialView("_ConfirmSpaceForCaravan");
                    
        }
        public ActionResult RentSpaceForTent()
        {
            return PartialView("_ReservedConfirmation");
        }
        public ActionResult RentSpaceForCaravan()
        {
            return PartialView("_ReservedConfirmation");
        }
    }
}