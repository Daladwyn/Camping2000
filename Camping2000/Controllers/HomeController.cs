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
            Booking newBooking = new Booking();
            using (var context = new Camping2000Db())
            {
                campingSpot = context.Camping.FirstOrDefault(i => i.ItemName == "Camping Spot1");
            }
            newBooking.BookingStartDate = DateTime.Now;
            newBooking.BookingEndDate = DateTime.Now.AddDays(1);
            newBooking.BookingPrice = campingSpot.CampingPrice;
            ViewBag.Message = "Renting space for tent.";
            return PartialView("_SpaceForTent", newBooking);
        }
        public ActionResult SpaceForCaravan()
        {
            Camping campingSpot = new Camping();
            Booking newBooking = new Booking();
            using (var context = new Camping2000Db())
            {
                campingSpot = context.Camping.FirstOrDefault(i => i.ItemName == "Camping Spot1");
            }
            newBooking.BookingStartDate = DateTime.Now;
            newBooking.BookingEndDate = DateTime.Now.AddDays(1);
            newBooking.BookingPrice = campingSpot.CampingPrice;
            ViewBag.Message = "Renting space for caravan.";
            return PartialView("_SpaceForCaravan", newBooking);
        }
        public ActionResult RentSpaceForTent([Bind(Include = "BookingStartDate,BookingEndDate,NumberOfGuests,BookingNeedsElectricity")] Booking newBooking)//missing data ItemId, GuestId, Price, Bookingid
        {
            if (ModelState.IsValid)
            {
                int numberOfDays = 0;
                decimal estimatedPrice = 0;
                List<Booking> currentBookings = new List<Booking>();
                List<int> eligibleSpots = new List<int>();
                List<int> notEligibleSpots = new List<int>();
                List<Camping> ListOfSpots = new List<Camping>();

                ViewBag.Errormessage = "";
                using (var context = new Camping2000Db()) //Gather all present bookings in a list.
                {
                    foreach (var booking in context.Bookings)
                    {
                        currentBookings.Add(booking);
                    }
                }
                if (newBooking.BookingNeedsElectricity == true) //Gather spots based on if electricity is needed as price differs
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
                    newBooking.BookingPrice = ListOfSpots[0].CampingPrice;
                }
                else if (newBooking.BookingNeedsElectricity == false)//Gather spots based on if electricity is  not needed as price differs
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
                    newBooking.BookingPrice = ListOfSpots[0].CampingPrice;
                }

                if (currentBookings.Capacity != 0) //check if any bookings is present
                {
                    foreach (var booking in currentBookings)
                    {
                        if ((newBooking.BookingStartDate >= booking.BookingEndDate) || (newBooking.BookingEndDate <= booking.BookingStartDate))//if the new bookings startdate is between the booked spots start and end date
                        {
                            eligibleSpots.Add(booking.ItemId);
                        }
                        else
                        {
                            notEligibleSpots.Add(booking.ItemId);
                        }
                    }
                }
                else //if no bookings is present choose the first spot 
                {
                    newBooking.ItemId = ListOfSpots[0].ItemId;
                }
                eligibleSpots.Sort();
                notEligibleSpots.Sort();

                if (ListOfSpots.Capacity < notEligibleSpots.Capacity)//Performance wise complete iterate on spots not acceptable first 
                {
                    for (int i = ListOfSpots.Count - 1; i >= 0; i--)
                    {
                        for (int y = notEligibleSpots.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[i].ItemId == notEligibleSpots[y])
                            {
                                ListOfSpots.Remove(ListOfSpots[i]);
                            }
                        }
                    }
                }
                else //Performance wise complete iterate on spots acceptable first 
                {
                    for (int i = notEligibleSpots.Count - 1; i >= 0; i--)
                    {
                        for (int y = ListOfSpots.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[y].ItemId == notEligibleSpots[i])
                            {
                                ListOfSpots.Remove(ListOfSpots[y]);
                            }
                        }
                    }
                }
                if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                {
                    ViewBag.Errormessage = "There is no available space for you. Please choose another arrivaldate and departuredate.";
                    return PartialView("_SpaceForTent", newBooking);
                }
                newBooking.ItemId = ListOfSpots[0].ItemId;

                //Calculate the price for the guest
                numberOfDays = newBooking.BookingEndDate.DayOfYear - newBooking.BookingStartDate.DayOfYear;
                estimatedPrice = newBooking.BookingPrice * numberOfDays * newBooking.NumberOfGuests;
                newBooking.BookingPrice = estimatedPrice;
                using (var context = new Camping2000Db()) //Save to database current info
                {
                    context.Bookings.Add(newBooking);
                    int checkDbSave = context.SaveChanges();
                    if (checkDbSave < 1)
                    {
                        ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                        return PartialView("_SpaceForTent", newBooking);
                    }
                }
                return PartialView("_ConfirmSpaceForTent", newBooking);
            }
            else
            {
                return PartialView("_SpaceForTent", newBooking);
            }
        }
        public ActionResult RentSpaceForCaravan()
        {
            return PartialView("_ConfirmSpaceForCaravan");
        }
        public ActionResult ConfirmSpaceForTent([Bind(Include = "BookingId,GuestId")] Booking newBooking)
        {
            Booking inCompleteBooking = new Booking();
            ViewBag.Errormessage = "";
            using (var context = new Camping2000Db())
            {
                Guest presentGuest = new Guest();
                inCompleteBooking = context.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                inCompleteBooking.GuestId = newBooking.GuestId;
                presentGuest = context.Guests.SingleOrDefault(i => i.GuestId == newBooking.GuestId);
                presentGuest.GuestHasToPay = presentGuest.GuestHasToPay + inCompleteBooking.BookingPrice;
                presentGuest.GuestHasReserved = true;
                int checkDbSave = context.SaveChanges();
                if (checkDbSave < 1)
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_ConfirmSpaceForTent", newBooking);
                }
            }
            return PartialView("_ReservedConfirmation", newBooking);
        }
        public ActionResult ConfirmSpaceForCaravan()
        {
            return PartialView("_ReservedConfirmation");
        }
        public ActionResult PrintReservation()
        {
            return PartialView("Index");
        }
        [Authorize(Roles = "Administrators,Receptionist")]
        public ActionResult CheckIn()
        {
            Camping2000Db Db = new Camping2000Db();
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> presentDayArrivals = new List<Booking>();
            List<Guest> presentDayGuest = new List<Guest>();
            List<Camping> presentDaySpot = new List<Camping>();
            List<BookingGuestViewModel> presentDayBookings = new List<BookingGuestViewModel>();
            foreach (var booking in allBookings)
            {
                if (booking.BookingStartDate == DateTime.Now.Date)//is this correct Dateformat?
                {
                    presentDayArrivals.Add(booking);
                }
            }
            foreach (var booking in presentDayArrivals)
            {
                presentDayGuest.Add(Db.Guests.Find(booking.GuestId));
                presentDaySpot.Add(Db.Camping.Find(booking.ItemId));
            }
            for (int i = 0; i < presentDayArrivals.Count; i++)
            {
                presentDayBookings.Add(new BookingGuestViewModel
                {
                    BookingId = presentDayArrivals[i].BookingId,
                    BookingPrice = presentDayArrivals[i].BookingPrice,
                    //ItemId = presentDayArrivals[i].ItemId,
                    ItemName = presentDaySpot[i].ItemName,
                    NumberOfGuests = presentDayArrivals[i].NumberOfGuests,
                    GuestFirstName = presentDayGuest[i].GuestFirstName,
                    GuestLastName = presentDayGuest[i].GuestLastName

                });



                //presentDayBookings[i].BookingId = presentDayArrivals[i].BookingId;
                //presentDayBookings[i].BookingPrice = presentDayArrivals[i].BookingPrice;
                //presentDayBookings[i].ItemId = presentDayArrivals[i].ItemId;
                //presentDayBookings[i].NumberOfGuests = presentDayArrivals[i].NumberOfGuests;
                //presentDayBookings[i].GuestFirstName = presentDayGuest[i].GuestFirstName;
                //presentDayBookings[i].GuestLastName = presentDayGuest[i].GuestLastName;
            }


            return PartialView("_CheckIn", presentDayBookings);
        }
        [Authorize(Roles = "Administrators, Receptionist")]
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
        public ActionResult CheckInConfirmation(BookingGuestViewModel checkInBooking, int NumberOfCheckInGuests)
        {
            Camping2000Db Db = new Camping2000Db();
            Booking booking = Db.Bookings.SingleOrDefault(i => i.BookingId == checkInBooking.BookingId);
            int newAmountOfGuests = 0;
            if (booking.NumberOfGuests < NumberOfCheckInGuests) //Check if number of guests differ from reservation
            {
                newAmountOfGuests = NumberOfCheckInGuests - booking.NumberOfGuests;
            }
            else if (booking.NumberOfGuests > NumberOfCheckInGuests)
            {
                newAmountOfGuests = booking.NumberOfGuests - NumberOfCheckInGuests;
            }
            else
            {

            }

                return PartialView("_CheckInConfirmation",booking);
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
        public ActionResult ShowGuestArrivals(BookingGuestViewModel arrivals)
        {
           
            return PartialView("_ShowGuestArrivals", arrivals);
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
        [Authorize(Roles = "Administrators")]
        public ActionResult ShowVacantSpots()
        {
            List<Camping> vacantSpots = new List<Camping>();
            using (var context = new Camping2000Db())
            {
                foreach (var spot in context.Camping)
                {
                    if (spot.ItemIsBooked == false)
                    {
                        vacantSpots.Add(spot);
                    }
                }
            }
            if (vacantSpots == null)
            {
                return PartialView("_NoAvailableSpotToChangeTo");
            }
            return PartialView("_ShowVacantSpots", vacantSpots);
        }
    }
}