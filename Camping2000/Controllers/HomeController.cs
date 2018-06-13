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
        public ActionResult RentSpaceForTent()
        {
            return PartialView("_ConfirmSpaceForTent");
        }
        public ActionResult RentSpaceForCaravan()
        {
            return PartialView("_ConfirmSpaceForCaravan");
        }
        public ActionResult ConfirmSpaceForTent()
        {
            return PartialView("_ReservedConfirmation");
        }
        public ActionResult ConfirmSpaceForCaravan()
        {
            return PartialView("_ReservedConfirmation");
        }
        public ActionResult PrintReservation()
        {
            return PartialView("Index");
        }
        public ActionResult CheckIn()
        {
            return PartialView("_CheckIn");
        }
        public ActionResult CheckOut()
        {
            return PartialView("_CheckOut");
        }
        public ActionResult ArrivalsDepartures()
        {
            return PartialView("_ArrivalsDepartures");
        }
        public ActionResult CheckInConfirmation()
        {
            return PartialView("_CheckInConfirmation");
        }
        public ActionResult CheckOutConfirmation()
        {
            return PartialView("_CheckOutConfirmation");
        }
        public ActionResult GoToStart()
        {
            return PartialView("Index");
        }
        public ActionResult ShowGuestArrivals()
        {
            return PartialView("_ShowGuestArrivals");
        }
        public ActionResult ShowGuestDepartures() 
            {
            return PartialView("_ShowGuestDepartures");
            }
        public ActionResult ModifyBooking()
        {
            return PartialView("_ModifyBooking");
        }
        public ActionResult ModifyGuestDetails()
        {
            return PartialView("_ModifyGuestDetails");
        }
        public ActionResult ListPresentBookings()
        {
            return PartialView("_PresentBooking");
        }
        public ActionResult ModifySpecificBooking()
        {
            return PartialView("_ModifySpecificBooking");
        }
        public ActionResult UpdatedBooking()
        {
            return PartialView("_UpdatedBooking");
        }
        public ActionResult SearchForGuest()
        {
            return PartialView("_ShowFoundGuests");
        }
        public ActionResult ModifySpecificGuestDetails()
        {
            return PartialView("_GuestDetails");
        }
        public ActionResult UpdatedGuestDetails()
        {
            return PartialView("_UpdatedGuestDetails");
        }
    }
}