using Camping2000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Camping2000.Controllers
{
    public class CheckInCheckOutController : Controller
    {
        private Camping2000Db Db;

        public CheckInCheckOutController()
        {
            Db = new Camping2000Db();
        }

        [Authorize(Roles = "Administrators,Receptionists")]
        public ActionResult CheckIn()
        {
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> presentDayArrivals = new List<Booking>();
            List<ApplicationUser> presentDayGuests = new List<ApplicationUser>();
            List<Camping> presentDaySpots = new List<Camping>();
            List<BookingGuestViewModel> presentDayBookings = new List<BookingGuestViewModel>();
            ViewBag.Errormessage = "";
            if (allBookings == null)//check if any data is received
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_FailedCheckIn");
            }
            foreach (var booking in allBookings)//check for arrivals on present day
            {
                if ((booking.BookingStartDate.ToShortDateString() == DateTime.Now.ToShortDateString()) && (booking.GuestHasReserved == true))
                {
                    presentDayArrivals.Add(booking);
                }
            }
            for (int i = 0; i < presentDayArrivals.Count(); i++) //Remove the Guests that already have checked in
            {
                if (presentDayArrivals[i].GuestHasCheckedIn == true)
                {
                    presentDayArrivals.Remove(presentDayArrivals[i]);
                    i--;
                }
            }
            if (presentDayArrivals.Count < 1) //if no arrivals is coming present day creata a message to user
            {
                ViewBag.Errormessage = "No arrivals today or all guest have arrived already!";
                return PartialView("_CheckIn");
            }
            foreach (var booking in presentDayArrivals) //gather guestdata and spotdata from database in separate lists
            {
                presentDayGuests.Add(Db.Users.Find(booking.GuestId));
                presentDaySpots.Add(Db.Camping.Find(booking.ItemId));
            }
            if ((presentDayArrivals.Count != presentDayGuests.Count) || (presentDayArrivals.Count != presentDaySpots.Count))//check that matching data is present.
            {
                ViewBag.Errormessage = "Matching data is missing. Please try again.";
                return PartialView("_FailedCheckIn");
            }
            for (int i = 0; i < presentDayArrivals.Count; i++) //join the data into a data viewmodel
            {
                presentDayBookings.Add(new BookingGuestViewModel
                {
                    BookingId = presentDayArrivals[i].BookingId,
                    BookingPrice = presentDayArrivals[i].BookingPrice,
                    ItemId = presentDayArrivals[i].ItemId,
                    ItemName = presentDaySpots[i].ItemName,
                    NumberOfGuests = presentDayArrivals[i].NumberOfGuests,
                    GuestId = presentDayGuests[i].GuestId,
                    GuestFirstName = presentDayGuests[i].GuestFirstName,
                    GuestLastName = presentDayGuests[i].GuestLastName
                });
            }
            return PartialView("_CheckIn", presentDayBookings);
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult CheckInConfirmation(int BookingId, int NumberOfCheckInGuests)
        {
            if (ModelState.IsValid)
            {
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == BookingId);
                Camping currentSpot = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == currentBooking.GuestId);
                int numberOfDays = 0;
                if ((currentBooking == null) || (currentSpot == null) || (currentGuest == null))//check if fetched data is valid
                {
                    ViewBag.Errormessage = "Fetching of data did not succeed. Please try again.";
                    return PartialView("_FailedCheckIn");
                }
                BookingGuestViewModel checkInBooking = new BookingGuestViewModel
                {
                    BookingId = currentBooking.BookingId,
                    BookingPrice = currentBooking.BookingPrice,
                    GuestFirstName = currentGuest.GuestFirstName,
                    GuestLastName = currentGuest.GuestLastName,
                    GuestId = currentGuest.GuestId,
                    ItemId = currentSpot.ItemId,
                    ItemName = currentSpot.ItemName,
                    NumberOfGuests = currentBooking.NumberOfGuests
                };
                if (currentBooking.NumberOfGuests != NumberOfCheckInGuests) //Check if PartySize differ from reservation
                {
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    currentBooking.BookingPrice = currentSpot.CampingPrice * NumberOfCheckInGuests * numberOfDays;
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                    currentBooking.GuestHasCheckedIn = true;
                    currentBooking.GuestHasReserved = false;
                    currentBooking.NumberOfGuests = NumberOfCheckInGuests;
                    currentSpot.ItemIsOccupied = true;
                    int numberOfSaves = Db.SaveChanges();
                    if (numberOfSaves != 3)
                    {
                        ViewBag.Errormessage = "The Checkin failed! Please check the number of checked in persons and the cost for the stay.";
                        return PartialView("_FailedCheckIn", checkInBooking);
                    }
                    checkInBooking.BookingPrice = currentBooking.BookingPrice;
                    checkInBooking.NumberOfGuests = NumberOfCheckInGuests;
                    return PartialView("_CheckInConfirmation", checkInBooking);
                }
                else
                {
                    currentBooking.GuestHasCheckedIn = true;
                    currentBooking.GuestHasReserved = false;
                    currentSpot.ItemIsOccupied = true;
                    int numberOfSaves = Db.SaveChanges();
                    if (numberOfSaves != 2)
                    {
                        ViewBag.Errormessage = "The Checkin failed! Please check the number of checked in persons and the cost for the stay.";
                        return PartialView("_FailedCheckIn", checkInBooking);
                    }
                    return PartialView("_CheckInConfirmation", checkInBooking);
                }
            }
            else
            {
                ViewBag.Errormessage = "The check out did not recive correct indata. Please try again.";
                return PartialView("_FailedCheckIn");
            }
        }
        
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult CheckOut()
        {
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> departingGuestBookings = new List<Booking>();
            List<ApplicationUser> departingGuests = new List<ApplicationUser>();
            List<Camping> Campingpots = new List<Camping>();
            List<BookingGuestViewModel> presentDepartingBookings = new List<BookingGuestViewModel>();
            ViewBag.Errormessage = "";
            if (allBookings == null)//check if any data is recieved
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_Checkout", presentDepartingBookings);
            }
            foreach (var booking in allBookings)
            {
                if ((booking.BookingEndDate.ToShortDateString() == DateTime.Now.ToShortDateString()) && (booking.GuestHasCheckedIn == true) && (booking.BookingIsPaid == false))
                {
                    departingGuestBookings.Add(booking);
                }
            }
            if (departingGuestBookings.Count() < 1)
            {
                ViewBag.Errormessage = "No checkouts are planned to be made today or all departing guests have already checked out.";
                return PartialView("_CheckOut");
            }
            foreach (var booking in departingGuestBookings) //gather guestdata and spotdata from database in separate lists
            {
                departingGuests.Add(Db.Users.Find(booking.GuestId));
                Campingpots.Add(Db.Camping.Find(booking.ItemId));
            }
            if ((departingGuestBookings.Count != departingGuests.Count) || (departingGuestBookings.Count != Campingpots.Count))
            {
                ViewBag.Errormessage = "Matching data is missing. Please try again.";
                return PartialView("_CheckOut");
            }
            for (int i = 0; i < departingGuestBookings.Count; i++) //join the data into a data viewmodel
            {
                presentDepartingBookings.Add(new BookingGuestViewModel
                {
                    BookingId = departingGuestBookings[i].BookingId,
                    BookingPrice = departingGuestBookings[i].BookingPrice,
                    ItemId = departingGuestBookings[i].ItemId,
                    ItemName = Campingpots[i].ItemName,
                    NumberOfGuests = departingGuestBookings[i].NumberOfGuests,
                    GuestId = departingGuests[i].GuestId,
                    GuestFirstName = departingGuests[i].GuestFirstName,
                    GuestLastName = departingGuests[i].GuestLastName
                });
            }
            return PartialView("_CheckOut", presentDepartingBookings);
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutConfirmation([Bind(Include = "BookingId,NumberOfGuests")]BookingGuestViewModel checkingOutGuest)
        {
            if (ModelState.IsValid)
            {
                List<Booking> allBookings = Db.Bookings.ToList();
                Booking departingBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == checkingOutGuest.BookingId);
                ApplicationUser departingGuest = Db.Users.SingleOrDefault(i => i.GuestId == departingBooking.GuestId);
                Camping departedGuestSpot = Db.Camping.SingleOrDefault(i => i.ItemId == departingBooking.ItemId);
                List<Booking> allGuestBookings = new List<Booking>();
                BookingGuestViewModel departingGuestBooking = new BookingGuestViewModel();
                List<LinkBooking> allLinkBookings = Db.LinkBookings.ToList();
                if ((allBookings == null) || (allLinkBookings == null)) //check if any data is received
                {
                    ViewBag.Errormessage = "Fetching of data did not succeed. Please try again.";
                    return PartialView("_CheckOut");
                }
                if ((departingBooking == null) || (departedGuestSpot == null) || (departingGuest == null))//check if fetched data is valid
                {
                    ViewBag.Errormessage = "No booking, spot or guest could be found. Please try again.";
                    return PartialView("_CheckOut");
                }
                foreach (var booking in allBookings) //collect any other bookings the guest have made
                {
                    if ((booking.GuestId == departingGuest.GuestId) && ((booking.GuestHasCheckedIn == true) || booking.GuestHasReserved == true))
                    {
                        allGuestBookings.Add(booking);
                    }
                }
                if (allGuestBookings.Count == 1)//if no other bookings exist
                {
                    departingGuest.GuestHasCheckedIn = false;
                    departingGuest.GuestHasReserved = false;
                    departingGuest.GuestHasPaid = departingGuest.GuestHasPaid + departingBooking.BookingPrice;
                    departingGuest.GuestHasToPay = departingGuest.GuestHasToPay - departingBooking.BookingPrice;
                    departingBooking.GuestHasCheckedIn = false;
                    departingBooking.GuestHasReserved = false;
                    departingBooking.BookingIsPaid = true;
                    departedGuestSpot.ItemIsOccupied = false;
                    if (Db.SaveChanges() != 3)
                    {
                        ViewBag.Errormessage = "The check out did partly succed.";
                        return PartialView("_CheckOutConfirmation", checkingOutGuest);
                    }
                }
                else //if more bookings exist handle the specific booking.
                {
                    departingGuest.GuestHasPaid = departingGuest.GuestHasPaid + departingBooking.BookingPrice;
                    departingGuest.GuestHasToPay = departingGuest.GuestHasToPay - departingBooking.BookingPrice;
                    departingBooking.GuestHasCheckedIn = false;
                    departingBooking.GuestHasReserved = false;
                    departingBooking.BookingIsPaid = true;
                    departedGuestSpot.ItemIsOccupied = false;
                }
                int numberOfSaves = Db.SaveChanges();
                if (numberOfSaves != 3)
                {
                    ViewBag.Errormessage = "The check out did partly succed.";
                    return PartialView("_FailedCheckOut", checkingOutGuest);
                }
                departingGuestBooking.BookingId = departingBooking.BookingId;
                departingGuestBooking.GuestFirstName = departingGuest.GuestFirstName;
                departingGuestBooking.GuestLastName = departingGuest.GuestLastName;
                departingGuestBooking.BookingPrice = departingBooking.BookingPrice;
                departingGuestBooking.NumberOfGuests = departingBooking.NumberOfGuests;
                departingGuestBooking.ItemName = departedGuestSpot.ItemName;
                return PartialView("_CheckOutConfirmation", departingGuestBooking);
            }
            else
            {
                ViewBag.Errormessage = "The check out did not recive correct indata. Please try again.";
                return PartialView("_FailedCheckOut", checkingOutGuest);
            }
        }
        //End of checkout flow
        //Start of Arrival/departures daily flow
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult ArrivalsDepartures()
        {
            List<ModifyBookingViewModel> arrivalsDepartures = new List<ModifyBookingViewModel>();
            List<Booking> allBookings = Db.Bookings.ToList();
            Booking currentBooking = new Booking();
            ApplicationUser currentGuest = new ApplicationUser();
            Camping currentItem = new Camping();
            if (allBookings == null)//check if any data is recieved
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_ArrivalsDepartures");
            }
            foreach (var booking in allBookings)
            {
                if ((booking.BookingStartDate.ToShortDateString() == DateTime.Now.ToShortDateString()) || (booking.BookingEndDate.ToShortDateString() == DateTime.Now.ToShortDateString()))
                {
                    if ((booking.GuestHasReserved == true) || (booking.GuestHasCheckedIn == true))
                    {
                        if ((booking.GuestId != null) && (booking.ItemId != 0))
                        {
                            currentGuest = Db.Users.SingleOrDefault(i => i.Id == booking.GuestId);
                            currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == booking.ItemId);
                            arrivalsDepartures.Add(new ModifyBookingViewModel
                            {
                                BookingId = booking.BookingId,
                                BookingStartDate = booking.BookingStartDate,
                                BookingEndDate = booking.BookingEndDate,
                                GuestId = booking.GuestId,
                                ItemId = booking.ItemId,
                                ItemName = currentItem.ItemName,
                                NumberOfGuests = booking.NumberOfGuests,
                                BookingNeedsElectricity = booking.BookingNeedsElectricity,
                                GuestFirstName = currentGuest.GuestFirstName,
                                GuestLastName = currentGuest.GuestLastName,
                                GuestHasCheckedIn = booking.GuestHasCheckedIn
                            });
                        }
                    }
                }
            }
            return PartialView("_ArrivalsDepartures", arrivalsDepartures);
        }
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult ShowGuestArrivals(BookingGuestViewModel arrivals)
        {
            return PartialView("_ShowGuestArrivals", arrivals);
        }
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult ShowGuestDepartures(BookingGuestViewModel departures)
        {
            return PartialView("_ShowGuestDepartures", departures);
        }
    }
}