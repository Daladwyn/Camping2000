﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Camping2000.Models;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Camping2000.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpaceForTent([Bind(Include = "GuestId,BookingId,BookingStartDate,BookingEndDate,BookingNeedsElectricity,NumberOfGuests")]Booking newBooking)
        {
            Camping2000Db Db = new Camping2000Db();
            Camping campingSpot = Db.Camping.FirstOrDefault(i => i.CampingElectricity == newBooking.BookingNeedsElectricity);
            if (newBooking.BookingId == 0)//if guestId is null its a new reservation
            {
                newBooking.BookingNeedsElectricity = newBooking.BookingNeedsElectricity;
                newBooking.BookingStartDate = DateTime.Now;
                newBooking.BookingEndDate = DateTime.Now.AddDays(1);
                newBooking.BookingPrice = campingSpot.CampingPrice;
                return PartialView("_SpaceForTent", newBooking);
            }
            else //if GuestId is not null then the guest wants to change some data in reservation
            {
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                newBooking.BookingPrice = currentBooking.BookingPrice;
                return PartialView("_SpaceForTent", newBooking);
            }
        }
        public ActionResult RentSpaceForTent([Bind(Include = "BookingStartDate,BookingEndDate,NumberOfGuests,BookingNeedsElectricity,BookingId,GuestId")]Booking newBooking)//missing data ItemId, GuestId, Price, Bookingid
        {
            if (ModelState.IsValid)
            {
                int numberOfDays = 0;
                Camping2000Db Db = new Camping2000Db();
                List<Booking> currentBookings = Db.Bookings.ToList();//Gather all present bookings in a list.
                List<int> notEligibleSpots = new List<int>();//list of invalid spotnumbers
                List<Camping> ListOfSpots = new List<Camping>();//list of valid spots
                ViewBag.Errormessage = "";
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
                foreach (var spot in Db.Camping)//Gather spots based on if electricity is needed as price differs
                {
                    if (spot.CampingElectricity == newBooking.BookingNeedsElectricity)
                    {
                        ListOfSpots.Add(spot);
                    }
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
                    for (int i = ListOfSpots.Count - 1; i >= 0; i--)//remove occupied spots from the list of spots
                    {
                        for (int y = notEligibleSpots.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[i].ItemId == notEligibleSpots[y])
                            {
                                ListOfSpots.Remove(ListOfSpots[i]);
                                i--;
                            }
                        }
                    }
                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There is no available space for you. Please choose another arrivaldate and departuredate.";
                        return PartialView("_SpaceForTent", newBooking);
                    }
                    newBooking.ItemId = ListOfSpots[0].ItemId;
                }
                numberOfDays = CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate); //Calculate number of days
                newBooking.BookingPrice = ListOfSpots[0].CampingPrice * numberOfDays * newBooking.NumberOfGuests;//Calculate the price for the guest
                if (newBooking.BookingId == 0)//Save to database current info if a new reservation and not a readjusted reservation
                {
                    Db.Bookings.Add(newBooking);
                }
                int checkDbSave = Db.SaveChanges();
                if (checkDbSave < 1) //Check is database save was ok
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_SpaceForTent", newBooking);
                }
                return PartialView("_ConfirmSpaceForTent", newBooking);
            }
            else
            {
                return PartialView("_SpaceForTent", newBooking);//return previous view as indata is invalid
            }
        }
        public ActionResult ConfirmSpaceForTent([Bind(Include = "BookingId,GuestId")]Booking newBooking)
        //public ActionResult ConfirmSpaceForTent(int BookingId, string GuestId, int NumberOfGuests)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                ViewBag.Errormessage = "";
                Booking inCompleteBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                Guest presentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == newBooking.GuestId);
                inCompleteBooking.GuestId = presentGuest.GuestId;
                presentGuest.GuestHasToPay = presentGuest.GuestHasToPay + inCompleteBooking.BookingPrice;
                inCompleteBooking.GuestHasReserved = true;
                inCompleteBooking.GuestHasCheckedIn = false;
                int checkDbSave = Db.SaveChanges();
                if (checkDbSave < 2)
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_ConfirmSpaceForTent", newBooking);
                }
                return PartialView("_ReservedConfirmation", newBooking);
            }
            else
            {
                ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                return PartialView("_ConfirmSpaceForTent", newBooking);
            }
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
            List<Guest> presentDayGuests = new List<Guest>();
            List<Camping> presentDaySpots = new List<Camping>();
            List<BookingGuestViewModel> presentDayBookings = new List<BookingGuestViewModel>();
            ViewBag.Errormessage = "";
            foreach (var booking in allBookings)//check for arrivals on present day
            {
                if ((booking.BookingStartDate.ToShortDateString() == DateTime.Now.ToShortDateString()) && (booking.GuestHasReserved == true))
                {
                    presentDayArrivals.Add(booking);
                }
            }
            if (presentDayArrivals.Count < 1) //if no arrivals is coming present day creata a message to user
            {
                ViewBag.Errormessage = "No arrivals today!";
                return PartialView("_CheckIn");
            }
            foreach (var booking in presentDayArrivals) //gather guestdata and spotdata from database in separate lists
            {
                presentDayGuests.Add(Db.Guests.Find(booking.GuestId));
                presentDaySpots.Add(Db.Camping.Find(booking.ItemId));
            }
            for (int i = 0; i < presentDayArrivals.Count(); i++) //Remove the Guest that already have checked in
            {
                if (presentDayArrivals[i].GuestHasCheckedIn == true)
                {
                    presentDayGuests.Remove(presentDayGuests[i]);
                    presentDayArrivals.Remove(presentDayArrivals[i]);
                    presentDaySpots.Remove(presentDaySpots[i]);
                    i--;
                }
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
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult CheckOut()
        {
            Camping2000Db Db = new Camping2000Db();
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> departingGuestBooking = new List<Booking>();
            List<Guest> departingGuest = new List<Guest>();
            List<Camping> campingSpot = new List<Camping>();
            List<BookingGuestViewModel> presentDepartingBookings = new List<BookingGuestViewModel>();
            ViewBag.Errormessage = "";
            foreach (var booking in allBookings)
            {
                if ((booking.BookingEndDate.ToShortDateString() == DateTime.Now.ToShortDateString()) && (booking.GuestHasCheckedIn == true))
                {
                    departingGuestBooking.Add(booking);
                }
            }
            if (departingGuestBooking.Count() < 1)
            {
                ViewBag.Errormessage = "No checkouts are planned to be made today or all departing guests have already checked out.";
                return PartialView("_CheckOut", presentDepartingBookings);
            }
            foreach (var booking in departingGuestBooking) //gather guestdata and spotdata from database in separate lists
            {
                departingGuest.Add(Db.Guests.Find(booking.GuestId));
                campingSpot.Add(Db.Camping.Find(booking.ItemId));
            }
            for (int i = 0; i < departingGuestBooking.Count; i++) //join the data into a data viewmodel
            {
                presentDepartingBookings.Add(new BookingGuestViewModel
                {
                    BookingId = departingGuestBooking[i].BookingId,
                    BookingPrice = departingGuestBooking[i].BookingPrice,
                    ItemId = departingGuestBooking[i].ItemId,
                    ItemName = campingSpot[i].ItemName,
                    NumberOfGuests = departingGuestBooking[i].NumberOfGuests,
                    GuestId = departingGuest[i].GuestId,
                    GuestFirstName = departingGuest[i].GuestFirstName,
                    GuestLastName = departingGuest[i].GuestLastName
                });
            }
            return PartialView("_CheckOut", presentDepartingBookings);
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult ArrivalsDepartures()
        {
            Camping2000Db Db = new Camping2000Db();
            List<ModifyBookingViewModel> arrivalsDepartures = new List<ModifyBookingViewModel>();
            List<Booking> allBookings = Db.Bookings.ToList();
            Booking currentBooking = new Booking();
            Guest currentGuest = new Guest();
            Camping currentItem = new Camping();
            foreach (var booking in allBookings)
            {
                if ((booking.BookingStartDate.ToShortDateString() == DateTime.Now.ToShortDateString()) || (booking.BookingEndDate.ToShortDateString() == DateTime.Now.ToShortDateString()))
                {
                    if (booking.GuestId != null)
                    {
                        currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == booking.GuestId);
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
                    };
                }
            }
            return PartialView("_ArrivalsDepartures", arrivalsDepartures);
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult CheckInConfirmation(int BookingId, int NumberOfCheckInGuests)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                Booking booking = Db.Bookings.SingleOrDefault(i => i.BookingId == BookingId);
                Camping spotThatIsReserved = Db.Camping.SingleOrDefault(i => i.ItemId == booking.ItemId);
                Guest guestThatHaveReserved = Db.Guests.SingleOrDefault(i => i.GuestId == booking.GuestId);
                int numberOfDays = 0;
                BookingGuestViewModel checkInBooking = new BookingGuestViewModel
                {
                    BookingId = booking.BookingId,
                    BookingPrice = booking.BookingPrice,
                    GuestFirstName = guestThatHaveReserved.GuestFirstName,
                    GuestLastName = guestThatHaveReserved.GuestLastName,
                    GuestId = guestThatHaveReserved.GuestId,
                    ItemId = spotThatIsReserved.ItemId,
                    ItemName = spotThatIsReserved.ItemName,
                    NumberOfGuests = booking.NumberOfGuests
                };
                if (booking.NumberOfGuests != NumberOfCheckInGuests) //Check if PartySize differ from reservation
                {
                    guestThatHaveReserved.GuestHasToPay = guestThatHaveReserved.GuestHasToPay - booking.BookingPrice;
                    numberOfDays = CalculateNumberOfDays(booking.BookingStartDate, booking.BookingEndDate);
                    booking.BookingPrice = spotThatIsReserved.CampingPrice * NumberOfCheckInGuests * numberOfDays;
                    guestThatHaveReserved.GuestHasToPay = guestThatHaveReserved.GuestHasToPay + booking.BookingPrice;
                    booking.GuestHasCheckedIn = true;
                    booking.GuestHasReserved = false;
                    booking.NumberOfGuests = NumberOfCheckInGuests;
                    spotThatIsReserved.ItemIsBooked = true;
                    int numberOfSaves = Db.SaveChanges();
                    if (numberOfSaves != 3)
                    {
                        ViewBag.Errormessage = "The Checkin failed! Please check the number of checked in persons and the cost for the stay.";
                        return PartialView("_ShowGuestArrivals", checkInBooking);
                    }
                    checkInBooking.BookingPrice = booking.BookingPrice;
                    checkInBooking.NumberOfGuests = NumberOfCheckInGuests;
                    return PartialView("_CheckInConfirmation", checkInBooking);
                }
                else
                {
                    booking.GuestHasCheckedIn = true;
                    booking.GuestHasReserved = false;
                    spotThatIsReserved.ItemIsBooked = true;
                    int numberOfSaves = Db.SaveChanges();
                    if (numberOfSaves != 2)
                    {
                        ViewBag.Errormessage = "The Checkin failed! Please check the number of checked in persons and the cost for the stay.";
                        return PartialView("_ShowGuestArrivals", checkInBooking);
                    }
                    return PartialView("_CheckInConfirmation", checkInBooking);
                }
            }
            else
            {
                Camping2000Db Db = new Camping2000Db();
                Booking booking = Db.Bookings.SingleOrDefault(i => i.BookingId == BookingId);
                Camping spotThatIsReserved = Db.Camping.SingleOrDefault(i => i.ItemId == booking.ItemId);
                Guest guestThatHaveReserved = Db.Guests.SingleOrDefault(i => i.GuestId == booking.GuestId);
                BookingGuestViewModel checkInBooking = new BookingGuestViewModel
                {
                    BookingId = booking.BookingId,
                    BookingPrice = booking.BookingPrice,
                    GuestFirstName = guestThatHaveReserved.GuestFirstName,
                    GuestLastName = guestThatHaveReserved.GuestLastName,
                    GuestId = guestThatHaveReserved.GuestId,
                    ItemId = spotThatIsReserved.ItemId,
                    ItemName = spotThatIsReserved.ItemName,
                    NumberOfGuests = booking.NumberOfGuests
                };
                return PartialView("_Checkin", checkInBooking);
            }
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult CheckOutConfirmation([Bind(Include = "BookingId")]BookingGuestViewModel checkingOutGuest)
        {
            Camping2000Db Db = new Camping2000Db();
            Booking departingBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == checkingOutGuest.BookingId);
            Guest departingGuest = Db.Guests.SingleOrDefault(i => i.GuestId == departingBooking.GuestId);
            Camping departedGuestSpot = Db.Camping.SingleOrDefault(i => i.ItemId == departingBooking.ItemId);
            List<Booking> allGuestBookings = new List<Booking>();
            BookingGuestViewModel departingGuestBooking = new BookingGuestViewModel();
            foreach (var booking in Db.Bookings) //collect any other bookings the guest have made
            {
                if ((booking.GuestId == departingGuest.GuestId) && (booking.GuestHasCheckedIn == true))
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
                departedGuestSpot.ItemIsBooked = false;
            }
            else //if more bookings exist handle the specific booking.
            {
                departingGuest.GuestHasPaid = departingGuest.GuestHasPaid + departingBooking.BookingPrice;
                departingGuest.GuestHasToPay = departingGuest.GuestHasToPay - departingBooking.BookingPrice;
                departingBooking.GuestHasCheckedIn = false;
                departingBooking.GuestHasReserved = false;
                departedGuestSpot.ItemIsBooked = false;
            }
            int numberOfSaves = Db.SaveChanges();
            if (numberOfSaves != 3)
            {
                ViewBag.Errormessage = "The check out did not succed.";
                return PartialView("_CheckOut", checkingOutGuest);
            }
            departingGuestBooking.BookingId = departingBooking.BookingId;
            departingGuestBooking.GuestFirstName = departingGuest.GuestFirstName;
            departingGuestBooking.GuestLastName = departingGuest.GuestLastName;
            departingGuestBooking.BookingPrice = departingBooking.BookingPrice;
            departingGuestBooking.NumberOfGuests = departingBooking.NumberOfGuests;
            departingGuestBooking.ItemName = departedGuestSpot.ItemName;
            return PartialView("_CheckOutConfirmation", departingGuestBooking);
        }
        public ActionResult GoToStart()
        {
            return PartialView("Index");
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult ShowGuestArrivals(BookingGuestViewModel arrivals)
        {
            return PartialView("_ShowGuestArrivals", arrivals);
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult ShowGuestDepartures(BookingGuestViewModel departures)
        {
            return PartialView("_ShowGuestDepartures", departures);
        }
        [Authorize(Roles = "Administrators, Receptionist")]
        public ActionResult ModifyBooking()
        {
            Camping2000Db Db = new Camping2000Db();
            List<Booking> allBookings = Db.Bookings.ToList();
            List<BookingGuestViewModel> presentGuestBookings = new List<BookingGuestViewModel>();
            List<Booking> presentBookings = new List<Booking>();
            List<Guest> presentGuests = new List<Guest>();
            List<Camping> presentSpots = new List<Camping>();
            ViewBag.Errormessage = "";
            if (allBookings == null)
            {
                presentGuestBookings = null;
                ViewBag.Errormessage = "No bookings at all are available to modify.";
                return PartialView("_ModifyBooking", presentGuestBookings);
            }
            for (int i = 0; i < allBookings.Count; i++)
            {
                if ((allBookings[i].BookingEndDate >= DateTime.Now.Date) && ((allBookings[i].GuestHasReserved == true) || (allBookings[i].GuestHasCheckedIn == true)))
                {
                    presentBookings.Add(allBookings[i]);
                }
            }
            if (presentBookings.Count < 1)
            {
                presentGuestBookings = null;
                ViewBag.Errormessage = "No current bookings are available to modify.";
                return PartialView("_ModifyBooking", presentGuestBookings);
            }
            foreach (var booking in presentBookings)
            {
                presentGuests.Add(Db.Guests.SingleOrDefault(i => i.GuestId == booking.GuestId));
                presentSpots.Add(Db.Camping.SingleOrDefault(i => i.ItemId == booking.ItemId));
            }

            for (int i = 0; i < presentBookings.Count; i++)
            {
                presentGuestBookings.Add(new BookingGuestViewModel
                {
                    BookingId = presentBookings[i].BookingId,
                    BookingPrice = presentBookings[i].BookingPrice,
                    GuestFirstName = presentGuests[i].GuestFirstName,
                    GuestLastName = presentGuests[i].GuestLastName,
                    GuestId = presentGuests[i].GuestId,
                    ItemId = presentSpots[i].ItemId,
                    ItemName = presentSpots[i].ItemName,
                    NumberOfGuests = presentBookings[i].NumberOfGuests
                });
            }
            return PartialView("_ModifyBooking", presentGuestBookings);
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifyGuestDetails()
        {
            return PartialView("_ModifyGuestDetails");
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ListPresentBookings(BookingGuestViewModel AGuestBooking)
        {
            return PartialView("_PresentBooking", AGuestBooking);
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifySpecificBooking([Bind(Include = "BookingId,ItemId,GuestId")] ModifyBookingViewModel aBookingToModify)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == aBookingToModify.BookingId);
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == currentBooking.GuestId);
                Camping currentSpot = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                List<Camping> allSpots = Db.Camping.ToList();
                List<Camping> freeSpots = new List<Camping>();
                foreach (var spot in allSpots)
                {
                    if (spot.ItemIsBooked == false)
                    {
                        freeSpots.Add(spot);
                    }
                }
                if (freeSpots.Count < 1)
                {
                    freeSpots = null;
                }
                ModifyBookingViewModel bookingToModify = new ModifyBookingViewModel
                {
                    BookingId = currentBooking.BookingId,
                    GuestId = currentGuest.GuestId,
                    GuestFirstName = currentGuest.GuestFirstName,
                    GuestLastName = currentGuest.GuestLastName,
                    GuestHasCheckedIn = currentBooking.GuestHasCheckedIn,
                    ItemId = currentSpot.ItemId,
                    ItemName = currentSpot.ItemName,
                    BookingStartDate = currentBooking.BookingStartDate,
                    BookingEndDate = currentBooking.BookingEndDate,
                    NumberOfGuests = currentBooking.NumberOfGuests,
                    BookingPrice = currentBooking.BookingPrice,
                    BookingNeedsElectricity = currentBooking.BookingNeedsElectricity,
                    VacantSpots = freeSpots
                };
                return PartialView("_ModifySpecificBooking", bookingToModify);
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", aBookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult SearchForGuest(string firstName, string lastName)
        {
            Camping2000Db Db = new Camping2000Db();
            List<Guest> foundGuests = new List<Guest>();
            string errormessage = "";
            if ((firstName == "") && (lastName == ""))
            {
                ViewBag.Errormessage = "Please specify the guests name before searching.";
                return PartialView("_ShowFoundGuests", foundGuests);
            }
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            if ((firstName != "") && (lastName == ""))
            {
                foreach (var guest in Db.Guests)
                {
                    if (guest.GuestFirstName.ToLower() == firstName)
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            else if ((firstName != "") && (lastName != ""))
            {
                foreach (var guest in Db.Guests)
                {
                    if ((guest.GuestFirstName.ToLower() == firstName) && (guest.GuestLastName.ToLower() == lastName))
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            else if ((firstName == "") && (lastName != ""))
            {
                foreach (var guest in Db.Guests)
                {
                    if (guest.GuestLastName.ToLower() == lastName)
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            if ((firstName != "") && (foundGuests.Count < 1))
            {
                errormessage = $"A guest with the name of {firstName} was not found. Please try again.";
                ViewBag.Errormessage = errormessage;
            }
            else if ((lastName != "") && (foundGuests.Count < 1))
            {
                errormessage = $"A guest with the name of {lastName} was not found. Please try again.";
                ViewBag.Errormessage = errormessage;
            }
            return PartialView("_ShowFoundGuests", foundGuests);
        }
        //[Authorize(Roles = "Administrators, Guests")]
        //[AllowAnonymous]
        public ActionResult ModifySpecificGuestDetails([Bind(Include = "GuestId")]Guest searchedGuest)
        {
            Camping2000Db Db = new Camping2000Db();
            Guest foundGuest = Db.Guests.SingleOrDefault(i => i.GuestId == searchedGuest.GuestId);
            Adress foundAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == searchedGuest.GuestId);
            GuestAdressViewModel completeGuestDetails = new GuestAdressViewModel
            {
                GuestId = foundGuest.GuestId,
                GuestFirstName = foundGuest.GuestFirstName,
                GuestLastName = foundGuest.GuestLastName,
                GuestNationality = foundGuest.GuestNationality,
                GuestMobileNumber = foundGuest.GuestMobileNumber,
                GuestPhoneNumber = foundGuest.GuestPhoneNumber,
                AdressId = foundAdress.AdressId,
                PostAdressCity = foundAdress.PostAdressCity,
                PostAdressStreet1 = foundAdress.PostAdressStreet1,
                PostAdressStreet2 = foundAdress.PostAdressStreet2,
                PostAdressStreet3 = foundAdress.PostAdressStreet3,
                PostAdressZipCode = foundAdress.PostAdressZipCode,
                LivingAdressCity = foundAdress.LivingAdressCity,
                LivingAdressStreet1 = foundAdress.LivingAdressStreet1,
                LivingAdressStreet2 = foundAdress.LivingAdressStreet2,
                LivingAdressStreet3 = foundAdress.LivingAdressStreet3,
                LivingAdressZipCode = foundAdress.LivingAdressZipCode
            };
            return PartialView("_GuestDetails", completeGuestDetails);
        }
        [Authorize(Roles = "Administrators, Guests")]
        public ActionResult UpdatedGuestDetails([Bind(Include = "GuestFirstName,GuestLastName,GuestNationality,GuestPhoneNumber,GuestMobileNumber," +
            "GuestId,LivingAdressStreet1,LivingAdressStreet2,LivingAdressStreet3,LivingAdressZipCode,LivingAdressCity," +
            "PostAdressStreet1,PostAdressStreet2,PostAdressStreet3,PostAdressZipCode,PostAdressCity")] GuestAdressViewModel newGuestData)
        {
            Camping2000Db Db = new Camping2000Db();
            Guest oldGuestData = Db.Guests.SingleOrDefault(i => i.GuestId == newGuestData.GuestId);
            Adress oldGuestAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == newGuestData.GuestId);
            oldGuestData.GuestFirstName = newGuestData.GuestFirstName;
            oldGuestData.GuestLastName = newGuestData.GuestLastName;
            oldGuestData.GuestNationality = newGuestData.GuestNationality;
            oldGuestData.GuestPhoneNumber = newGuestData.GuestPhoneNumber;
            oldGuestData.GuestMobileNumber = newGuestData.GuestMobileNumber;
            oldGuestAdress.LivingAdressStreet1 = newGuestData.LivingAdressStreet1;
            oldGuestAdress.LivingAdressStreet2 = newGuestData.LivingAdressStreet2;
            oldGuestAdress.LivingAdressStreet3 = newGuestData.LivingAdressStreet3;
            oldGuestAdress.LivingAdressZipCode = newGuestData.LivingAdressZipCode;
            oldGuestAdress.LivingAdressCity = newGuestData.LivingAdressCity;
            oldGuestAdress.PostAdressStreet1 = newGuestData.PostAdressStreet1;
            oldGuestAdress.PostAdressStreet2 = newGuestData.PostAdressStreet2;
            oldGuestAdress.PostAdressStreet3 = newGuestData.PostAdressStreet3;
            oldGuestAdress.PostAdressZipCode = newGuestData.PostAdressZipCode;
            oldGuestAdress.PostAdressCity = newGuestData.PostAdressCity;
            Db.SaveChanges();

            return PartialView("_UpdatedGuestDetails", newGuestData);
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
        [AllowAnonymous]
        public ActionResult GuestData()
        {
            Camping2000Db CampingDb = new Camping2000Db();
            ApplicationDbContext Db = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(Db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            List<string> listOfAppGuests = new List<string>();
            List<string> listOfCampingGuests = new List<string>();
            GuestDataViewModel newGuest = new GuestDataViewModel();
            foreach (var guest in Db.Users)
            {
                if (guest.Email != "admin@camping.com")
                {
                    listOfAppGuests.Add(guest.Id);
                }
            }
            foreach (var guest in CampingDb.Guests)
            {
                listOfCampingGuests.Add(guest.GuestId);
            }
            listOfAppGuests.Sort();
            listOfCampingGuests.Sort();
            for (int i = 0; i < listOfAppGuests.Count; i++)
            {
                if ((listOfCampingGuests.Count < 1) && (listOfAppGuests.Count == listOfCampingGuests.Count + 1))
                {
                    newGuest.GuestId = listOfAppGuests[0];
                    return PartialView("_GuestData", newGuest);
                }
                if (listOfAppGuests[i] != listOfCampingGuests[i])
                {
                    newGuest.GuestId = listOfAppGuests[i];
                    return PartialView("_GuestData", newGuest);
                }
                else
                {
                    listOfAppGuests.Remove(listOfAppGuests[i]);
                    listOfCampingGuests.Remove(listOfCampingGuests[i]);
                    i--;
                }
            }
            return PartialView("_GuestData", newGuest);
        }
        public ActionResult SaveGuestData([Bind(Include = "GuestFirstName,GuestLastName,GuestNationality,GuestPhoneNumber,GuestMobileNumber," +
            "GuestId,PostAdressStreet1,PostAdressStreet2,PostAdressStreet3,PostAdressZipCode,PostAdressCity," +
            "LivingAdressStreet1,LivingAdressStreet2,LivingAdressStreet3,LivingAdressZipCode,LivingAdressCity")]GuestDataViewModel newGuest)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                Guest guestData = new Guest();
                Adress guestAdress = new Adress();

                Camping2000.Models.ApplicationDbContext context = new ApplicationDbContext();
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindById(newGuest.GuestId); //find the new guest by the Id
                userManager.AddToRole(user.Id, "Guests");//add the guest to the role of "Guests"
                context.SaveChanges();

                guestData.GuestId = newGuest.GuestId;
                guestData.GuestFirstName = newGuest.GuestFirstName;
                guestData.GuestLastName = newGuest.GuestLastName;
                guestData.GuestNationality = newGuest.GuestNationality;
                guestData.GuestPhoneNumber = newGuest.GuestPhoneNumber;
                guestData.GuestMobileNumber = newGuest.GuestMobileNumber;
                guestData.GuestHasReserved = false;
                guestData.GuestHasCheckedIn = false;
                guestData.GuestHasPaid = 0;
                guestData.GuestHasToPay = 0;
                guestAdress.GuestId = newGuest.GuestId;
                guestAdress.LivingAdressStreet1 = newGuest.LivingAdressStreet1;
                guestAdress.LivingAdressStreet2 = newGuest.LivingAdressStreet2;
                guestAdress.LivingAdressStreet3 = newGuest.LivingAdressStreet3;
                guestAdress.LivingAdressZipCode = newGuest.LivingAdressZipCode;
                guestAdress.LivingAdressCity = newGuest.LivingAdressCity;
                guestAdress.PostAdressStreet1 = newGuest.PostAdressStreet1;
                guestAdress.PostAdressStreet2 = newGuest.PostAdressStreet2;
                guestAdress.PostAdressStreet3 = newGuest.PostAdressStreet3;
                guestAdress.PostAdressZipCode = newGuest.PostAdressZipCode;
                guestAdress.PostAdressCity = newGuest.PostAdressCity;
                Db.Guests.Add(guestData);
                Db.Adresses.Add(guestAdress);
                int numberOfSaves = Db.SaveChanges();
                if (numberOfSaves < 2)
                {
                    ViewBag.Errormessage = "The registration did not complete. please try again in a while.";
                    return PartialView("_GuestData", newGuest);
                }
                return PartialView("_RegistrationComplete");
            }
            else
            {
                return PartialView("_GuestData", newGuest);
            }
        }
        public ActionResult GuestDetails(string GuestId)
        {
            Camping2000Db Db = new Camping2000Db();
            Guest foundGuest = Db.Guests.SingleOrDefault(i => i.GuestId == GuestId);
            if (foundGuest == null)
            {
                ViewBag.Errormessage = "No user data was found. Are you the admin or receptionist?";
                return PartialView("_FailedGuestDetails");
            }
            else
            {
                Adress foundAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == GuestId);
                GuestAdressViewModel completeGuestDetails = new GuestAdressViewModel
                {
                    GuestId = foundGuest.GuestId,
                    GuestFirstName = foundGuest.GuestFirstName,
                    GuestLastName = foundGuest.GuestLastName,
                    GuestNationality = foundGuest.GuestNationality,
                    GuestMobileNumber = foundGuest.GuestMobileNumber,
                    GuestPhoneNumber = foundGuest.GuestPhoneNumber,
                    AdressId = foundAdress.AdressId,
                    PostAdressCity = foundAdress.PostAdressCity,
                    PostAdressStreet1 = foundAdress.PostAdressStreet1,
                    PostAdressStreet2 = foundAdress.PostAdressStreet2,
                    PostAdressStreet3 = foundAdress.PostAdressStreet3,
                    PostAdressZipCode = foundAdress.PostAdressZipCode,
                    LivingAdressCity = foundAdress.LivingAdressCity,
                    LivingAdressStreet1 = foundAdress.LivingAdressStreet1,
                    LivingAdressStreet2 = foundAdress.LivingAdressStreet2,
                    LivingAdressStreet3 = foundAdress.LivingAdressStreet3,
                    LivingAdressZipCode = foundAdress.LivingAdressZipCode
                };
                return PartialView("_GuestDetails", completeGuestDetails);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangeStartDate([Bind(Include = "BookingId,GuestId,ItemId,BookingStartDate")] ModifyBookingViewModel bookingToModify)
        {
            Camping2000Db Db = new Camping2000Db();
            ModifyBookingViewModel currentBookingView = new ModifyBookingViewModel();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);

            List<Booking> bookingsWSCSpot = new List<Booking>(); //bookingsWithSameCampingSpot 
            List<Booking> bookingsWSCSECB = new List<Booking>(); //bookingsWithSameCampingSpotExcludingCurrentBooking 

            List<Booking> bookingsWithSameCampingSpot = new List<Booking>();
            List<int> disAllowableBookings = new List<int>();
            List<Camping> ListOfSpots = new List<Camping>();
            int numberOfDays = 0;
            //decimal estimatedPrice = 0;
            foreach (var booking in Db.Bookings) //gather all bookings that have the same Itemid as the present one 
            {
                if ((booking.ItemId == currentItem.ItemId) && (booking.BookingEndDate > bookingToModify.BookingStartDate) && (booking.BookingStartDate < bookingToModify.BookingStartDate))
                {
                    bookingsWithSameCampingSpot.Add(booking);
                }
            }
            foreach (var booking in bookingsWithSameCampingSpot)
            {
                if (booking.BookingId != currentBooking.BookingId)
                {
                    bookingsWSCSECB.Add(booking);
                }
            }
            if (bookingsWSCSECB.Count < 1)//If no other bookings exist change startday and booking price 
            {
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingStartDate = bookingToModify.BookingStartDate;
                numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                bookingToModify.NumberOfGuests = currentBooking.NumberOfGuests;
                bookingToModify.ItemName = currentItem.ItemName;
                Db.SaveChanges();
                ViewBag.Message = "The rescheduling of the startdate succeded with no change in placement.";
                return PartialView("_ChangeStartDate", bookingToModify);
            }
            else
            {
                ViewBag.Message = "A change of spot is needed.";
                foreach (var booking in Db.Bookings)//gather data of free spaces
                {
                    if ((booking.BookingEndDate > bookingToModify.BookingStartDate) || (booking.BookingStartDate < currentBooking.BookingEndDate))
                    {
                        disAllowableBookings.Add(booking.ItemId);
                    }
                }
                disAllowableBookings.Sort();
                if (currentBooking.BookingNeedsElectricity == true)
                {
                    foreach (var spot in Db.Camping)
                    {
                        if (spot.CampingElectricity == true)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
                else //Gather spots based on if electricity is  not needed as price differs
                {
                    foreach (var spot in Db.Camping)
                    {
                        if (spot.CampingElectricity == false)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
                if (ListOfSpots.Capacity < disAllowableBookings.Capacity)
                {
                    for (int i = ListOfSpots.Count - 1; i >= 0; i--)
                    {
                        for (int y = disAllowableBookings.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[i].ItemId == disAllowableBookings[y])
                            {
                                ListOfSpots.Remove(ListOfSpots[i]);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = disAllowableBookings.Count - 1; i >= 0; i--)
                    {
                        for (int y = ListOfSpots.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[y].ItemId == disAllowableBookings[i])
                            {
                                ListOfSpots.Remove(ListOfSpots[y]);
                            }
                        }
                    }
                }
                if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                {
                    ViewBag.Errormessage = "There are no available space for you. Please choose another arrivaldate.";
                    return PartialView("_FailedChangeStartDate", currentBooking);
                }
                currentBooking.ItemId = ListOfSpots[0].ItemId;
                currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                //Calculate the price for the guest
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingStartDate = bookingToModify.BookingStartDate;
                numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = currentItem.CampingPrice * numberOfDays * currentBooking.NumberOfGuests;
                Db.SaveChanges();
                bookingToModify.BookingStartDate = currentBooking.BookingStartDate;
                bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.NumberOfGuests = currentBooking.NumberOfGuests;
                return PartialView("_ChangeStartDate", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangeEndDate([Bind(Include = "BookingId,GuestId,ItemId,BookingEndDate")] ModifyBookingViewModel bookingToModify)
        {
            Camping2000Db Db = new Camping2000Db();
            ModifyBookingViewModel currentBookingView = new ModifyBookingViewModel();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
            List<Booking> allBookings = Db.Bookings.ToList();
            List<int> disAllowableBookings = new List<int>();
            List<Booking> bookingsWithSameCampingSpot = new List<Booking>();
            List<Camping> ListOfSpots = new List<Camping>();
            int numberOfDays = 0;
            foreach (var booking in allBookings)
            {
                if ((booking.ItemId == currentItem.ItemId) && (booking.BookingStartDate <= bookingToModify.BookingEndDate))
                {
                    bookingsWithSameCampingSpot.Add(booking);
                }
            }
            if (bookingsWithSameCampingSpot.Count < 1)//If no other bookings exist change endday and booking price
            {
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingEndDate = bookingToModify.BookingEndDate;
                numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                Db.SaveChanges();
                ViewBag.Message = "The rescheduling of the enddate succeded with no change in placement.";
                return PartialView("_ChangeEndDate", bookingToModify);
            }
            else
            {
                ViewBag.Message = "A change of spot is needed.";
                foreach (var booking in Db.Bookings) //gather data of free spaces
                {
                    if (((booking.GuestHasCheckedIn == true) || (booking.GuestHasReserved == true)) && ((booking.BookingEndDate > currentBooking.BookingStartDate) || (booking.BookingStartDate < currentBooking.BookingEndDate)))
                    {
                        disAllowableBookings.Add(booking.ItemId);
                    }
                }
                disAllowableBookings.Sort();
                if (currentBooking.BookingNeedsElectricity == true)
                {
                    foreach (var spot in Db.Camping)
                    {
                        if (spot.CampingElectricity == true)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
                else //Gather spots based on if electricity is  not needed as price differs
                {
                    foreach (var spot in Db.Camping)
                    {
                        if (spot.CampingElectricity == false)
                        {
                            ListOfSpots.Add(spot);
                        }
                    }
                }
                if (ListOfSpots.Capacity < disAllowableBookings.Capacity)
                {
                    for (int i = ListOfSpots.Count - 1; i >= 0; i--)
                    {
                        for (int y = disAllowableBookings.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[i].ItemId == disAllowableBookings[y])
                            {
                                ListOfSpots.Remove(ListOfSpots[i]);

                            }
                        }
                    }
                }
                else
                {
                    for (int i = disAllowableBookings.Count - 1; i >= 0; i--)
                    {
                        for (int y = ListOfSpots.Count - 1; y >= 0; y--)
                        {
                            if (ListOfSpots[y].ItemId == disAllowableBookings[i])
                            {
                                ListOfSpots.Remove(ListOfSpots[y]);
                                y--;
                            }
                        }
                    }
                }
                if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                {
                    ViewBag.Errormessage = "There are no available space for you. Please choose another departuredate.";
                    return PartialView("_FailedChangeEndDate", currentBooking);
                }
                currentBooking.ItemId = ListOfSpots[0].ItemId;
                currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                //Calculate the price for the guest
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingEndDate = bookingToModify.BookingEndDate;
                numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = currentItem.CampingPrice * numberOfDays * currentBooking.NumberOfGuests;
                Db.SaveChanges();
                bookingToModify.BookingStartDate = currentBooking.BookingStartDate;
                bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.NumberOfGuests = currentBooking.NumberOfGuests;
                return PartialView("_ChangeEndDate", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangePowerOutlet([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                List<ModifyBookingViewModel> currentBookingView = new List<ModifyBookingViewModel>();
                ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
                ModifyBookingViewModel anotherBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Booking> allBookings = Db.Bookings.ToList();
                List<int> disAllowableBookings = new List<int>();
                List<Booking> bookingsWithSameCampingSpot = new List<Booking>();
                List<Camping> ListOfSpots = new List<Camping>();
                int numberOfDays = 0;
                List<Booking> lb = new List<Booking>();
                if (currentBooking.GuestHasCheckedIn == false)
                {
                    currentBooking.BookingNeedsElectricity = (currentBooking.BookingNeedsElectricity == false) ? currentBooking.BookingNeedsElectricity = true : currentBooking.BookingNeedsElectricity = false; //switch the electrical needs of the booking.
                    foreach (var booking in Db.Bookings)//see if any bookings exits that collide with the current booking
                    {
                        if ((booking.BookingNeedsElectricity == currentBooking.BookingNeedsElectricity) && (((booking.BookingEndDate > currentBooking.BookingStartDate) && (booking.BookingEndDate <= currentBooking.BookingEndDate)) || ((booking.BookingStartDate < currentBooking.BookingEndDate) && (booking.BookingStartDate >= currentBooking.BookingStartDate))))
                        {
                            disAllowableBookings.Add(booking.ItemId);
                        }
                    }
                    disAllowableBookings.Sort();
                    if (currentBooking.BookingNeedsElectricity == true) //Gather spots based on electrical demand
                    {
                        foreach (var spot in Db.Camping)
                        {
                            if (spot.CampingElectricity == true)
                            {
                                ListOfSpots.Add(spot);
                            }
                        }
                    }
                    else //Gather spots based on if electricity is  not needed as price differs
                    {
                        foreach (var spot in Db.Camping)
                        {
                            if (spot.CampingElectricity == false)
                            {
                                ListOfSpots.Add(spot);
                            }
                        }
                    }

                    if (disAllowableBookings.Count < 1)//if no collision is detected and the guest is not checked in, change spot
                    {
                        currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                        numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate); ;
                        currentItem = ListOfSpots[0];
                        currentBooking.ItemId = currentItem.ItemId;
                        currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                        Db.SaveChanges();
                        lb.Add(currentBooking);
                        ViewBag.Message = "The change of poweroutlet succeded. See details below.";
                        return PartialView("_ChangeStartDate", lb);
                    }
                    if (ListOfSpots.Capacity < disAllowableBookings.Capacity)//Remove camping spots that colliding bookings is using.
                    {
                        for (int i = ListOfSpots.Count - 1; i >= 0; i--)
                        {
                            for (int y = disAllowableBookings.Count - 1; y >= 0; y--)
                            {
                                if (ListOfSpots[i].ItemId == disAllowableBookings[y])
                                {
                                    ListOfSpots.Remove(ListOfSpots[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = disAllowableBookings.Count - 1; i >= 0; i--)//Remove camping spots that colliding bookings is using.
                        {
                            for (int y = ListOfSpots.Count - 1; y >= 0; y--)
                            {
                                if (ListOfSpots[y].ItemId == disAllowableBookings[i])
                                {
                                    ListOfSpots.Remove(ListOfSpots[y]);
                                }
                            }
                        }
                    }
                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There are no available spots that matches your need for electricity.";
                        return PartialView("_FailedChangePowerOutlet", currentBooking);
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    currentBooking.ItemId = ListOfSpots[0].ItemId;
                    currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                    currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                    Db.SaveChanges();
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                    aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    aBookingView.ItemName = currentItem.ItemName;
                    aBookingView.BookingPrice = currentBooking.BookingPrice;
                    aBookingView.BookingId = currentBooking.BookingId;
                    aBookingView.GuestId = currentBooking.GuestId;
                    aBookingView.ItemId = currentItem.ItemId;
                    aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    currentBookingView.Add(aBookingView);
                    ViewBag.Message = "The change of poweroutlet succeded. See details below.";
                    return PartialView("_ChangePowerOutlet", currentBookingView);
                }
                else
                {
                    bookingToModify.BookingNeedsElectricity = (currentBooking.BookingNeedsElectricity == false) ? bookingToModify.BookingNeedsElectricity = true : bookingToModify.BookingNeedsElectricity = false; //switch the electrical needs of the booking.
                    bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                    bookingToModify.NumberOfGuests = currentBooking.NumberOfGuests;
                    currentBooking.BookingEndDate = DateTime.Now;
                    Booking newBooking = new Booking(); ;//initialize a new booking with the values of the present booking
                    newBooking.BookingStartDate = DateTime.Now;
                    newBooking.BookingEndDate = bookingToModify.BookingEndDate;
                    newBooking.GuestHasCheckedIn = true;
                    newBooking.GuestHasReserved = false;
                    newBooking.NumberOfGuests = bookingToModify.NumberOfGuests;
                    Db.Bookings.Add(newBooking);
                    Db.SaveChanges();
                    newBooking.BookingNeedsElectricity = bookingToModify.BookingNeedsElectricity;
                    newBooking.BookingPrice = 0;
                    newBooking.GuestId = bookingToModify.GuestId;
                    Db.SaveChanges();
                    foreach (var booking in Db.Bookings)//see if any bookings exits that collide with the current booking
                    {
                        if ((booking.BookingNeedsElectricity == currentBooking.BookingNeedsElectricity) && ((booking.BookingEndDate > currentBooking.BookingStartDate) || (booking.BookingStartDate < currentBooking.BookingEndDate)))
                        {
                            disAllowableBookings.Add(booking.ItemId);
                        }
                    }
                    disAllowableBookings.Sort();
                    if (bookingToModify.BookingNeedsElectricity == true) //Gather spots based on electrical demand
                    {
                        foreach (var spot in Db.Camping)
                        {
                            if (spot.CampingElectricity == true)
                            {
                                ListOfSpots.Add(spot);
                            }
                        }
                    }
                    else //Gather spots based on if electricity is  not needed as price differs
                    {
                        foreach (var spot in Db.Camping)
                        {
                            if (spot.CampingElectricity == false)
                            {
                                ListOfSpots.Add(spot);
                            }
                        }
                    }
                    if (ListOfSpots.Capacity < disAllowableBookings.Capacity)
                    {
                        for (int i = ListOfSpots.Count - 1; i >= 0; i--)
                        {
                            for (int y = disAllowableBookings.Count - 1; y >= 0; y--)
                            {
                                if (ListOfSpots[i].ItemId == disAllowableBookings[y])
                                {
                                    ListOfSpots.Remove(ListOfSpots[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = disAllowableBookings.Count - 1; i >= 0; i--)
                        {
                            for (int y = ListOfSpots.Count - 1; y >= 0; y--)
                            {
                                if (ListOfSpots[y].ItemId == disAllowableBookings[i])
                                {
                                    ListOfSpots.Remove(ListOfSpots[y]);
                                }
                            }
                        }
                    }
                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There are no available spots that matches your need for electricity.";
                        return PartialView("_FailedChangePowerOutlet", lb);
                    }
                    newBooking.ItemId = ListOfSpots[0].ItemId;
                    currentItem.ItemIsBooked = false;
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    if (numberOfDays == 0)
                    {
                        ViewBag.Errormessage = "The change of poweroutlet is on the same day as checkin day.";
                        currentBooking.BookingPrice = 0;
                    }
                    else
                    {
                        currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                    Db.SaveChanges();
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                    aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    aBookingView.ItemName = currentItem.ItemName;
                    aBookingView.BookingPrice = currentBooking.BookingPrice;
                    aBookingView.BookingId = currentBooking.BookingId;
                    aBookingView.GuestId = currentBooking.GuestId;
                    aBookingView.ItemId = currentItem.ItemId;
                    aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    currentBookingView.Add(aBookingView);
                    currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == newBooking.ItemId);
                    currentItem.ItemIsBooked = true;
                    numberOfDays = CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate);
                    if (numberOfDays == 0)
                    {
                        ViewBag.Errormessage = "The change of poweroutlet is on the same day as checkout day.";
                        newBooking.BookingPrice = 0;
                    }
                    else
                    {
                        newBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * newBooking.NumberOfGuests;
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + newBooking.BookingPrice;
                    Db.SaveChanges();
                    anotherBookingView.BookingEndDate = newBooking.BookingEndDate;
                    anotherBookingView.BookingStartDate = newBooking.BookingStartDate;
                    anotherBookingView.ItemName = currentItem.ItemName;
                    anotherBookingView.BookingPrice = newBooking.BookingPrice;
                    anotherBookingView.BookingId = newBooking.BookingId;
                    anotherBookingView.GuestId = newBooking.GuestId;
                    anotherBookingView.ItemId = currentItem.ItemId;
                    anotherBookingView.NumberOfGuests = newBooking.NumberOfGuests;
                    anotherBookingView.BookingNeedsElectricity = newBooking.BookingNeedsElectricity;
                    currentBookingView.Add(anotherBookingView);
                    ViewBag.Message = "The change of poweroutlet succeded. See details below.";
                    return PartialView("_ChangePowerOutlet", currentBookingView);
                }
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangePartySize([Bind(Include = "BookingId,GuestId,ItemId,NumberOfGuests")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                List<ModifyBookingViewModel> currentBookingView = new List<ModifyBookingViewModel>();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                List<Booking> lb = new List<Booking>();
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                int numberOfDays = 0;
                if (currentBooking.GuestHasCheckedIn == false)
                {
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    currentBooking.BookingPrice = numberOfDays * bookingToModify.NumberOfGuests * currentItem.CampingPrice;
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                    currentBooking.NumberOfGuests = bookingToModify.NumberOfGuests;
                    Db.SaveChanges();
                    lb.Add(currentBooking);
                    return PartialView("_ChangePartySize", lb);
                }
                else
                {
                    bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                    Booking newBooking = new Booking(); ;//initialize a new booking with the values of the present booking
                    newBooking.BookingStartDate = DateTime.Now;
                    newBooking.BookingEndDate = bookingToModify.BookingEndDate;
                    newBooking.GuestHasCheckedIn = true;
                    newBooking.GuestHasReserved = false;
                    newBooking.NumberOfGuests = bookingToModify.NumberOfGuests;
                    Db.Bookings.Add(newBooking);
                    Db.SaveChanges();
                    bookingToModify.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    newBooking.BookingNeedsElectricity = bookingToModify.BookingNeedsElectricity;
                    newBooking.BookingPrice = 0;
                    newBooking.GuestId = bookingToModify.GuestId;
                    newBooking.ItemId = bookingToModify.ItemId;
                    //newBooking.NumberOfGuests = bookingToModify.NumberOfGuests;
                    Db.SaveChanges();
                    currentBooking.BookingEndDate = DateTime.Now;   //change end date for present booking
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice; //subtract the present bookingprice for the guest to pay 
                    numberOfDays = CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate); //calculate number of days the present stay was
                    if (numberOfDays == 0)
                    {
                        currentBooking.BookingPrice = 0; //if guest changes number of persons before first night but after checkin
                    }
                    else
                    {
                        currentBooking.BookingPrice = numberOfDays * currentBooking.NumberOfGuests * currentItem.CampingPrice; //calculate the present bookingprce
                    }
                    lb.Add(currentBooking);
                    numberOfDays = CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate);  //calculate the days in the new booking
                    if (numberOfDays != 0)
                    {
                        newBooking.BookingPrice = numberOfDays * bookingToModify.NumberOfGuests * currentItem.CampingPrice;   //calculate the new bookings price
                    }
                    else
                    {
                        ViewBag.Errormessage = "Guest can not change partysize last booked day.";
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice + newBooking.BookingPrice; //update the amount the guest have to pay 
                    lb.Add(newBooking);
                    Db.SaveChanges();
                    return PartialView("_ChangePartySize", lb);
                }
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangeCampingSpot([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Camping> availableSpots = new List<Camping>();
                List<Booking> collidingBookings = new List<Booking>();
                List<Booking> collidingBookingsWPN = new List<Booking>();//collidingBookingsWithPowerNeed
                if (currentBooking.GuestHasCheckedIn == true)
                {
                    foreach (var spot in Db.Camping)
                    {
                        if ((spot.ItemIsBooked == false) && (spot.ItemId != currentItem.ItemId) && (spot.CampingElectricity == currentBooking.BookingNeedsElectricity))
                        {
                            availableSpots.Add(spot);
                        }
                    }
                    foreach (var booking in Db.Bookings) //detect colliding bookings
                    {
                        if (((booking.BookingEndDate > currentBooking.BookingStartDate) && (booking.BookingEndDate <= currentBooking.BookingEndDate)) || ((booking.BookingStartDate < currentBooking.BookingEndDate) && (booking.BookingStartDate >= currentBooking.BookingStartDate)))
                        {
                            collidingBookings.Add(booking);
                        }
                    }
                    foreach (var booking in collidingBookings) //remove colliding bookings that have other needs regarding power outlet
                    {
                        if (booking.BookingNeedsElectricity == currentBooking.BookingNeedsElectricity)
                        {
                            collidingBookingsWPN.Add(booking);
                        }
                    }
                    foreach (var booking in collidingBookingsWPN)// remove camping items that is part of the collision bookings. 
                    {
                        for (int i = 0; i < availableSpots.Count; i++)
                        {
                            if (availableSpots[i].ItemId == booking.ItemId)
                            {
                                availableSpots.Remove(availableSpots[i]);
                                i--;
                            }
                        }
                    }
                    aBookingView.ItemId = currentItem.ItemId;
                    aBookingView.ItemName = currentItem.ItemName;
                    aBookingView.BookingId = currentBooking.BookingId;
                    aBookingView.GuestId = currentGuest.GuestId;
                    aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                    aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    aBookingView.VacantSpots = availableSpots;
                    return PartialView("_ChangeCampingSpot", aBookingView);
                }
                else
                {
                    //inform that change of spot is only doable after checkin.
                    aBookingView.ItemId = currentItem.ItemId;
                    aBookingView.ItemName = currentItem.ItemName;
                    aBookingView.BookingId = currentBooking.BookingId;
                    aBookingView.GuestId = currentGuest.GuestId;
                    aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                    aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    //aBookingView.VacantSpots = availableSpots;
                    ViewBag.GuestHasCheckedIn = "false";
                    return PartialView("_ChangeCampingSpot", aBookingView);
                }
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CancelReservation([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                ModifyBookingViewModel currentBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                if (currentBooking == null)
                {
                    ViewBag.Errormessage = "The booking is already cancelled.";
                    return PartialView("_FailedCancelReservation", bookingToModify);
                }
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Booking> guestsOtherBookings = new List<Booking>();
                foreach (var booking in Db.Bookings)
                {
                    if ((booking.GuestId == bookingToModify.GuestId) && (booking.BookingId != currentBooking.BookingId))
                    {
                        guestsOtherBookings.Add(booking);
                    }
                }
                if ((currentBooking.GuestHasReserved == true) && (currentBooking.GuestHasCheckedIn == false))
                {
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    Db.Bookings.Remove(currentBooking);
                    Db.SaveChanges();
                }
                else
                {
                    ViewBag.Errormessage = "The Guest have already checked in.";
                    return PartialView("_FailedCancelReservation", bookingToModify);
                }
                if (guestsOtherBookings == null)
                {
                    currentGuest.GuestHasReserved = false;
                    Db.SaveChanges();
                }
                return PartialView("_CancelReservation", bookingToModify);
            }
            else
            {
                ViewBag.Errormessage = "Something happend that prevented the cancellation of the reservation. please try again.";
                return PartialView("_FailedCancelReservation", bookingToModify); //check this returnstatment....
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult ChangeChooseCampingSpot([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            Camping2000Db Db = new Camping2000Db();
            ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
            Camping oldItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
            if (ModelState.IsValid)
            {
                oldItem.ItemIsBooked = false;
                Db.SaveChanges();
                currentItem.ItemIsBooked = true;
                currentBooking.ItemId = currentItem.ItemId;
                Db.SaveChanges();
                aBookingView = bookingToModify;
                aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                aBookingView.ItemName = currentItem.ItemName;

                return PartialView("_ChangeConfirmationCampingSpot", aBookingView);
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        public ActionResult Logoff()
        {
            return RedirectToAction("Logoff", "Account");
        }
        /// <summary>
        /// calculates a span of days. Take leapyears into consideration.
        /// Returns a interger value.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        static int CalculateNumberOfDays(DateTime startDate, DateTime endDate)
        {
            int numberOfDays;
            if (startDate.Year == endDate.Year)
            {
                numberOfDays = endDate.DayOfYear - startDate.DayOfYear;
            }
            else
            {
                if (DateTime.IsLeapYear(startDate.Year))
                {
                    numberOfDays = 366 - startDate.DayOfYear;
                    numberOfDays = numberOfDays + endDate.DayOfYear;
                }
                else
                {
                    numberOfDays = 365 - startDate.DayOfYear;
                    numberOfDays = numberOfDays + endDate.DayOfYear;
                }
            }
            return numberOfDays;
        }
    }
}