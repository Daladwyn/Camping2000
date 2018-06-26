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
            Camping campingSpot = new Camping();

            using (var context = new Camping2000Db())
            {
                campingSpot = context.Camping.First();

            }
            ViewBag.Message = "Renting space for tent.";
            return PartialView("_SpaceForTent", campingSpot);
        }
        public ActionResult SpaceForCaravan()
        {
            ViewBag.Message = "Renting space for caravan.";
            return PartialView("_SpaceForCaravan");
        }
        public ActionResult RentSpaceForTent([Bind(Include = "BookingStartDate,BookingEndDate,NumberOfGuests")] Booking newBooking, string Electricity)// , decimal CampingPrice
        {
            List<Booking> currentBookings = new List<Booking>();
            List<int> eligibleSpots = new List<int>();
            List<int> notEligibleSpots = new List<int>();
            List<Camping> ListOfSpots = new List<Camping>();
            using (var context = new Camping2000Db())
            {
                foreach (var booking in context.Bookings)
                {
                    currentBookings.Add(booking);
                }
            }
            foreach (var booking in currentBookings)
            {
                if ((booking.BookingStartDate > newBooking.BookingEndDate) || (booking.BookingEndDate < newBooking.BookingStartDate))//if the new bookings startdate is between the booked spots start and end date
                {
                    eligibleSpots.Add(booking.ItemId);
                }
                else
                {
                    notEligibleSpots.Add(booking.ItemId);
                }
            }
            eligibleSpots.Sort();
            notEligibleSpots.Sort();

            if (Electricity == "Yes")
            {
                using (var context = new Camping2000Db())
                {
                    foreach (var spot in context.Camping)
                    {
                        if (spot.CampingElectricity == true)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
            }
            else if (Electricity == "No")
            {
                using (var context = new Camping2000Db())
                {
                    foreach (var spot in context.Camping)
                    {
                        if (spot.CampingElectricity == false)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
            }


            for (int i = 0; i < ListOfSpots.Capacity; i++)
            {
                for (int y = 0; y < notEligibleSpots.Capacity; y++)
                {
                    if (ListOfSpots[i].ItemId < notEligibleSpots[y])
                    {
                        eligibleSpots.Add(ListOfSpots[i].ItemId);
                        i++;
                        y--;
                    }
                    else if (ListOfSpots[i].ItemId == notEligibleSpots[y])
                    {
                        i++;
                    }
                    else if (ListOfSpots[i].ItemId > notEligibleSpots[y])
                    {

                    }
                }
                eligibleSpots.Add(ListOfSpots[i].ItemId);
            }
            eligibleSpots.Sort();

           

           

            if (eligibleSpots == null)
            {
                // print out that no vacant spots exits.
                return PartialView("_NoVacantSpot");
            }






            return PartialView("_ConfirmSpaceForTent", newBooking);
        }
        public ActionResult RentSpaceForCaravan()
        {
            return PartialView("_ConfirmSpaceForCaravan");
        }
        public ActionResult ConfirmSpaceForTent([Bind(Include = "BookingStartDate,BookingEndDate,NumberOfGuests,GuestId")] Booking newBooking, string Electricity)
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
            using (var context = new Camping2000Db())
            {
                foreach (var booking in context.Bookings)
                {
                    if (booking.BookingStartDate == DateTime.Now.Date)//is this correct Dateformat?
                    {
                        presentBookings.Add(booking);
                    }
                }
            };
            //context.Dispose()
            return PartialView("_CheckIn", presentBookings);
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckOut()
        {
            List<Booking> presentBookings = new List<Booking>();
            using (var context = new Camping2000Db())
            {
                foreach (var booking in context.Bookings)
                {
                    if (booking.BookingEndDate == DateTime.Now.Date)//is this correct Dateformat?
                    {
                        presentBookings.Add(booking);
                    }
                }
            };
            return PartialView("_CheckOut", presentBookings);
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
        public ActionResult ShowGuestArrivals(Booking Abooking)
        {
            //using (var context = new Camping2000Db())
            //{

            //}
            return PartialView("_ShowGuestArrivals", Abooking);
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