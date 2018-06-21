using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Camping2000.Models;
using System.Data.SqlClient;

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
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckIn()
        {
            List<Booking> presentBookings = new List<Booking>();
            using(var context = new context())
            {


            };
            return PartialView("_CheckIn",presentBookings);
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckOut()
        {
            return PartialView("_CheckOut");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ArrivalsDepartures()
        {
            return PartialView("_ArrivalsDepartures");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckInConfirmation()
        {
            return PartialView("_CheckInConfirmation");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckOutConfirmation()
        {
            return PartialView("_CheckOutConfirmation");
        }
        public ActionResult GoToStart()
        {
            return PartialView("Index");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ShowGuestArrivals()
        {
            return PartialView("_ShowGuestArrivals");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ShowGuestDepartures()
        {
            return PartialView("_ShowGuestDepartures");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifyBooking()
        {
            return PartialView("_ModifyBooking");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifyGuestDetails()
        {
            return PartialView("_ModifyGuestDetails");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ListPresentBookings()
        {
            return PartialView("_PresentBooking");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifySpecificBooking()
        {
            return PartialView("_ModifySpecificBooking");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult UpdatedBooking()
        {
            return PartialView("_UpdatedBooking");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult SearchForGuest()
        {
            return PartialView("_ShowFoundGuests");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifySpecificGuestDetails()
        {
            return PartialView("_GuestDetails");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult UpdatedGuestDetails()
        {
            return PartialView("_UpdatedGuestDetails");
        }
        public ActionResult ShowVacantFullsign()
        {
            return PartialView("ShowVacantFullSign");
        }
    }
}