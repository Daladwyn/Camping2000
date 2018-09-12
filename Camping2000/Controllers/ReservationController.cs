using Camping2000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Camping2000.Controllers
{
    public class ReservationController : Controller
    {
        private Camping2000Db Db;

        public ReservationController()
        {
            Db = new Camping2000Db();
        }

        public ActionResult SpaceForTent([Bind(Include = "BookingNeedsElectricity")]Booking newBooking)
        {
            Camping currentSpot = Db.Camping.FirstOrDefault(i => i.CampingElectricity == newBooking.BookingNeedsElectricity);
            newBooking.BookingStartDate = DateTime.Now;
            newBooking.BookingEndDate = DateTime.Now.AddDays(1);
            newBooking.BookingPrice = Booking.checkForNullReferenceException(currentSpot.CampingPrice);
            if (newBooking.BookingPrice == 0)
            {
                ViewBag.Errormessage = "Price for a Campingpot could not be fetched. Please try again.";
            }
            string pricePerNight = newBooking.BookingNeedsElectricity == true ?
                "Space without electricity costs "+newBooking.BookingPrice+" kr/Night/Person." :
                "Space with electricity costs " + newBooking.BookingPrice + " Kr/Night/Person.";
            ViewBag.pricePerNight = pricePerNight;
            return PartialView("_SpaceForTent", newBooking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RentSpaceForTent([Bind(Include = "BookingStartDate,BookingEndDate," +
            "NumberOfGuests,BookingNeedsElectricity,BookingId,GuestId")]Booking newBooking)
        {
            if (ModelState.IsValid)
            {
                int numberOfDays = 0;
                List<Booking> currentBookings = Db.Bookings.ToList();//Gather all present bookings in a list.
                List<int> notEligibleSpots = new List<int>();//list of invalid spotnumbers
                List<Camping> ListOfSpots = new List<Camping>();//list of valid spots
                Booking updatedBooking = new Booking();
                ViewBag.Errormessage = "";
                HttpCookie campingCookie = new HttpCookie("CampingCookie");
                if (currentBookings == null)
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return PartialView("_SpaceForTent", newBooking);
                }
                if (newBooking.BookingId != 0)//a reservation readjustment have to exclude its own reservation
                {
                    for (int i = currentBookings.Count - 1; i >= 0; i--)
                    {
                        if (currentBookings[i].BookingId == newBooking.BookingId)
                        {
                            currentBookings.Remove(currentBookings[i]);
                            newBooking.ItemId = 0;//
                        }
                    }
                }
                ListOfSpots = Booking.FetchCampingSpots(newBooking.BookingNeedsElectricity);
                if (ListOfSpots[0].CampingSpot == "NoData")
                {
                    ViewBag.Errormessage = "Fetching campingdata did not succeed. Please try again.";
                    return PartialView("_SpaceForTent", newBooking);
                }
                if (newBooking.BookingStartDate >= newBooking.BookingEndDate)//check the start and end dates so start is before end.
                {
                    ViewBag.Errormessage = "You must arrive before you can depart. Please choose another start and/or end date.";
                    newBooking.BookingPrice = ListOfSpots[0].CampingPrice;
                    return PartialView("_SpaceForTent", newBooking);
                }
                if (currentBookings.Capacity == 0) //check if any bookings is present
                {
                    newBooking.ItemId = ListOfSpots[0].ItemId;//if no bookings is present choose the first spot 
                }
                else
                {
                    foreach (var booking in currentBookings)//gather the spots that is reserved in other bookings during the new bookings timeframe.
                    {
                        if ((newBooking.BookingStartDate >= booking.BookingEndDate) || (newBooking.BookingEndDate <= booking.BookingStartDate))//if the new bookings startdate is between the booked spots start and end date
                        { }
                        else
                        {
                            notEligibleSpots.Add(booking.ItemId);
                        }
                    }
                    notEligibleSpots.Sort();
                    ListOfSpots = Camping.RemoveOccupiedSpots(ListOfSpots, notEligibleSpots);

                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There is no available space for you. Please choose another arrivaldate and departuredate.";
                        return PartialView("_SpaceForTent", newBooking);
                    }
                    newBooking.ItemId = ListOfSpots[0].ItemId;
                }
                numberOfDays = Booking.CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate); //Calculate number of days
                newBooking.BookingPrice = ListOfSpots[0].CampingPrice * numberOfDays * newBooking.NumberOfGuests;//Calculate the price for the guest
                if (newBooking.BookingId == 0)//Save to database current info if a new reservation and not a readjusted reservation
                {
                    Db.Bookings.Add(newBooking);
                }
                else //if a booking exist update the booking with the new values
                {
                    updatedBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                    updatedBooking.BookingStartDate = newBooking.BookingStartDate;
                    updatedBooking.BookingEndDate = newBooking.BookingEndDate;
                    updatedBooking.NumberOfGuests = newBooking.NumberOfGuests;
                    updatedBooking.BookingNeedsElectricity = newBooking.BookingNeedsElectricity;
                    updatedBooking.BookingPrice = newBooking.BookingPrice;
                    updatedBooking.ItemId = newBooking.ItemId;
                    Db.SaveChanges();
                }
                int checkDbSave = Db.SaveChanges();
                if ((checkDbSave < 1) && (newBooking.GuestId == null))//Check is database save was ok
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_SpaceForTent", newBooking);
                }
                //cookiedetails is set here
                campingCookie["BookingId"] = Convert.ToString(newBooking.BookingId);
                campingCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(campingCookie);
                return PartialView("_ConfirmSpaceForTent", newBooking);
            }
            else
            {
                ViewBag.Errormessage = "Some of your submited values were incorrect. Please try again later.";
                return PartialView("_SpaceForTent", newBooking);//return previous view as indata is invalid
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SpaceAdjustments([Bind(Include = "BookingId,GuestId")]Booking newBooking)
        {
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
            if (currentBooking == null)//check if fetched data is valid
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please write down your bookingId before trying again.";
                return PartialView("_SpaceForTent", newBooking);
            }
            currentBooking.GuestId = Booking.checkForGuestNullReferenceException(newBooking.GuestId);
            if (currentBooking.GuestId == "Invalidstring")//check if fetched data is valid
            {
                ViewBag.Errormessage = "Sent data(GuestId) did not match. Please write down your bookingId before trying again.";
                return PartialView("_SpaceForTent", newBooking);
            }
            Camping currentSpot = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
            if (currentSpot == null)//check if fetched data is valid
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please write down your bookingId before trying again.";
                return PartialView("_SpaceForTent", newBooking);
            }
            newBooking.BookingPrice = Booking.checkForNullReferenceException(currentSpot.CampingPrice);
            if (newBooking.BookingPrice == 0)//check if fetched data is valid
            {
                ViewBag.Errormessage = "Price for a Campingpot could not be fetched. Please write down your bookingId before trying again.";
                return PartialView("_SpaceForTent", currentBooking);
            }
            Db.SaveChanges();
            return PartialView("_SpaceForTent", currentBooking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmSpace([Bind(Include = "BookingId,GuestId")]Booking acceptedBooking)
        {
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == acceptedBooking.BookingId);
            if (currentBooking == null)//check if fetched data is valid
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please write down your bookingId before you try again.";
                return PartialView("_ConfirmSpaceForTent", acceptedBooking);
            }
            currentBooking.GuestId = Booking.checkForGuestNullReferenceException(acceptedBooking.GuestId);
            currentBooking.GuestHasReserved = true;
            Db.SaveChanges();
            return PartialView("_ReservedConfirmation", acceptedBooking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrintReservation([Bind(Include = "BookingId")]Booking currentBooking)
        {
            ApplicationUser currentGuest = new ApplicationUser();
            ViewBag.Errormessage = "";
            try
            {
                currentBooking = Db.Bookings.SingleOrDefault(b => b.BookingId == currentBooking.BookingId);
            }
            catch (NullReferenceException)
            {
                ViewBag.Errormessage = "No booking was found. Please contact the campingstaff.";
                currentBooking.BookingId = 0;
                currentBooking.BookingStartDate = DateTime.Now;
                currentBooking.BookingEndDate = DateTime.Now;
                currentBooking.NumberOfGuests = 0;
                currentBooking.BookingNeedsElectricity = false;
                currentBooking.BookingPrice = 0;
                return PartialView("_FailedToPrintReservation");
            }
            try
            {
                currentGuest = Db.Users.SingleOrDefault(i => i.Id == currentBooking.GuestId);
            }
            catch (NullReferenceException)
            {
                string errorMessage = "No guest was found in booking " + currentBooking.BookingId + ". Please contact the campingstaff.";
                ViewBag.Errormessage = errorMessage;
                currentGuest.GuestId = "";
                currentGuest.GuestFirstName = "";
                currentGuest.GuestLastName = "";
                return PartialView("_FailedToPrintReservation");
            }
            GuestBookingViewModel bookingToPrint = new GuestBookingViewModel
            {
                BookingId = currentBooking.BookingId,
                GuestFirstName = currentGuest.GuestFirstName,
                GuestLastName = currentGuest.GuestLastName,
                BookingStartDate = currentBooking.BookingStartDate,
                BookingEndDate = currentBooking.BookingEndDate,
                NumberOfGuests = currentBooking.NumberOfGuests,
                BookingNeedsElectricity = currentBooking.BookingNeedsElectricity,
                BookingPrice = currentBooking.BookingPrice,
                GuestId = currentGuest.Id
            };
            return PartialView("_PrintReservation", bookingToPrint);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEMail([Bind(Include = "BookingId")]Booking currentBooking)
        {

            return PartialView("_SentEMailConfirmation");
        }
    }
}