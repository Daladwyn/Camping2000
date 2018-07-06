using System;
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
                campingSpot = context.Camping.FirstOrDefault(i => i.ItemName == "Trailer Spot1");
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
                Camping2000Db Db = new Camping2000Db();
                List<Booking> currentBookings = Db.Bookings.ToList();//Gather all present bookings in a list.
                List<int> eligibleSpots = new List<int>();
                List<int> notEligibleSpots = new List<int>();
                List<Camping> ListOfSpots = new List<Camping>();
                ViewBag.Errormessage = "";

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
        public ActionResult RentSpaceForCaravan([Bind(Include = "BookingStartDate,BookingEndDate,NumberOfGuests,BookingNeedsElectricity")] Booking newBooking)
        {
            if (ModelState.IsValid)
            {
                int numberOfDays = 0;
                decimal estimatedPrice = 0;
                Camping2000Db Db = new Camping2000Db();
                List<Booking> currentBookings = Db.Bookings.ToList();//Gather all present bookings in a list.
                List<int> eligibleSpots = new List<int>();
                List<int> notEligibleSpots = new List<int>();
                List<Camping> ListOfSpots = new List<Camping>();
                ViewBag.Errormessage = "";

                if (newBooking.BookingNeedsElectricity == true)
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

                if ((currentBookings.Capacity != 0))//check if any bookings is present
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

                if (ListOfSpots.Capacity < notEligibleSpots.Capacity)
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
                else
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
                    return PartialView("_SpaceForCaravan", newBooking);
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
                        return PartialView("_SpaceForCaravan", newBooking);
                    }
                }
                return PartialView("_ConfirmSpaceForCaravan", newBooking);
            }
            else
            {
                return PartialView("_SpaceForCaravan", newBooking);
            }
        }
        public ActionResult ConfirmSpaceForTent([Bind(Include = "BookingId,GuestId")] Booking newBooking)
        {
            Booking inCompleteBooking = new Booking();
            ViewBag.Errormessage = "";
            Guest presentGuest = new Guest();
            using (var context = new Camping2000Db())
            {
                inCompleteBooking = context.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                inCompleteBooking.GuestId = newBooking.GuestId;
                presentGuest = context.Guests.SingleOrDefault(i => i.GuestId == newBooking.GuestId);
                presentGuest.GuestHasToPay = presentGuest.GuestHasToPay + inCompleteBooking.BookingPrice;
                inCompleteBooking.GuestHasReserved = true;
                inCompleteBooking.GuestHasCheckedIn = false;
                int checkDbSave = context.SaveChanges();
                if (checkDbSave < 1)
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_ConfirmSpaceForTent", newBooking);
                }
            }
            return PartialView("_ReservedConfirmation", newBooking);
        }
        public ActionResult ConfirmSpaceForCaravan([Bind(Include = "BookingId,GuestId")] Booking newBooking)
        {
            Booking inCompleteBooking = new Booking();
            ViewBag.Errormessage = "";
            Guest presentGuest = new Guest();
            using (var context = new Camping2000Db())
            {
                inCompleteBooking = context.Bookings.SingleOrDefault(i => i.BookingId == newBooking.BookingId);
                inCompleteBooking.GuestId = newBooking.GuestId;
                presentGuest = context.Guests.SingleOrDefault(i => i.GuestId == newBooking.GuestId);
                presentGuest.GuestHasToPay = presentGuest.GuestHasToPay + inCompleteBooking.BookingPrice;
                inCompleteBooking.GuestHasReserved = true;
                inCompleteBooking.GuestHasCheckedIn = false;
                int checkDbSave = context.SaveChanges();
                if (checkDbSave < 1)
                {
                    ViewBag.Errormessage = "Your booking could not be processed. Please try again later.";
                    return PartialView("_ConfirmSpaceForTent", newBooking);
                }
            }
            return PartialView("_ReservedConfirmation", newBooking);
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
            ViewBag.Errormessage = "";
            foreach (var booking in allBookings)//check for arrivals on present day
            {
                if ((booking.BookingStartDate == DateTime.Now.Date) && (booking.GuestHasReserved == true))
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
                presentDayGuest.Add(Db.Guests.Find(booking.GuestId));
                presentDaySpot.Add(Db.Camping.Find(booking.ItemId));
            }
            for (int i = 0; i < presentDayArrivals.Count(); i++)
            {
                if (presentDayArrivals[i].GuestHasCheckedIn == true)
                {
                    presentDayGuest.Remove(presentDayGuest[i]);
                    presentDayArrivals.Remove(presentDayArrivals[i]);
                    presentDaySpot.Remove(presentDaySpot[i]);
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
                    ItemName = presentDaySpot[i].ItemName,
                    NumberOfGuests = presentDayArrivals[i].NumberOfGuests,
                    GuestId = presentDayGuest[i].GuestId,
                    GuestFirstName = presentDayGuest[i].GuestFirstName,
                    GuestLastName = presentDayGuest[i].GuestLastName

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
                if ((booking.BookingEndDate == DateTime.Now.Date) && (booking.GuestHasCheckedIn == true))
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
        [Authorize(Roles = "Administrators")]
        public ActionResult ArrivalsDepartures()
        {
            Camping2000Db Db = new Camping2000Db();
            List<ModifyBookingViewModel> arrivalsDepartures = new List<ModifyBookingViewModel>();
            Guest aGuest = new Guest();
            foreach (var booking in Db.Bookings)
            {
                if ((booking.BookingStartDate == DateTime.Now.Date) || (booking.BookingEndDate == DateTime.Now.Date))
                {
                    aGuest = Db.Guests.SingleOrDefault(i => i.GuestId == booking.GuestId);
                    arrivalsDepartures.Add(new ModifyBookingViewModel
                    {
                        BookingId = booking.BookingId,
                        BookingStartDate = booking.BookingStartDate,
                        BookingEndDate = booking.BookingEndDate,
                        GuestId = booking.GuestId,
                        ItemId = booking.ItemId,
                        NumberOfGuests = booking.NumberOfGuests,
                        BookingNeedsElectricity = booking.BookingNeedsElectricity,
                        GuestFirstName = aGuest.GuestFirstName,
                        GuestLastName = aGuest.GuestLastName,
                        GuestHasCheckedIn = booking.GuestHasCheckedIn

                    });
                }
            }
            return PartialView("_ArrivalsDepartures", arrivalsDepartures);
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckInConfirmation(int BookingId, int NumberOfCheckInGuests)
        {
            if (ModelState.IsValid)
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
                if (booking.NumberOfGuests != NumberOfCheckInGuests) //Check if number of guests differ from reservation
                {
                    guestThatHaveReserved.GuestHasToPay = guestThatHaveReserved.GuestHasToPay - booking.BookingPrice;
                    booking.BookingPrice = spotThatIsReserved.CampingPrice * NumberOfCheckInGuests;
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
        [Authorize(Roles = "Administrators")]
        public ActionResult CheckOutConfirmation(BookingGuestViewModel checkingOutGuest)
        {
            Camping2000Db Db = new Camping2000Db();
            Booking departingGuestBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == checkingOutGuest.BookingId);
            Guest departingGuest = Db.Guests.SingleOrDefault(i => i.GuestId == departingGuestBooking.GuestId);
            Camping departedGuestSpot = Db.Camping.SingleOrDefault(i => i.ItemId == departingGuestBooking.ItemId);
            List<Booking> otherGuestBookings = new List<Booking>();
            //otherGuestBookings.Add(Db.Bookings.Where( (i => i.GuestId == departingGuest.GuestId));
            foreach (var booking in Db.Bookings) //collect any other bookings the guest have made
            {
                if (booking.GuestId == departingGuest.GuestId)
                {
                    otherGuestBookings.Add(booking);
                }
            }
            if (otherGuestBookings.Count < 2)
            {
                departingGuest.GuestHasCheckedIn = false;
                departingGuest.GuestHasReserved = false;
                departingGuest.GuestHasPaid = departingGuest.GuestHasPaid + departingGuest.GuestHasToPay;
                departingGuestBooking.GuestHasCheckedIn = false;
                departingGuestBooking.GuestHasReserved = false;
                departedGuestSpot.ItemIsBooked = false;
            }
            else
            {
                departingGuest.GuestHasCheckedIn = true;
                departingGuest.GuestHasReserved = false;
                departingGuest.GuestHasPaid = departingGuest.GuestHasPaid + departingGuest.GuestHasToPay;
                departingGuestBooking.GuestHasCheckedIn = false;
                departingGuestBooking.GuestHasReserved = false;
                departedGuestSpot.ItemIsBooked = false;
            }
            int numberOfSaves = Db.SaveChanges();
            if (numberOfSaves != 3)
            {
                ViewBag.Errormessage = "The check out did not succed.";
                return PartialView("_CheckOut", checkingOutGuest);
            }
            return PartialView("_CheckOutConfirmation", departingGuest);
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
        public ActionResult ShowGuestDepartures(BookingGuestViewModel departures)
        {
            return PartialView("_ShowGuestDepartures", departures);
        }
        [Authorize(Roles = "Administrators")]
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
        public ActionResult ModifySpecificBooking(int bookingId)
        {
            Camping2000Db Db = new Camping2000Db();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingId);
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
        [Authorize(Roles = "Administrators")]
        public ActionResult UpdatedBooking([Bind(Include = "BookingStartDate,BookingEndDate,BookingNeedsElectricity," +
            "NumberOfGuests,BookingId,ItemId,GuestId")]ModifyBookingViewModel modifiedBookingDetail, int NewSpot)
        {
            if (ModelState.IsValid)
            {
                Camping2000Db Db = new Camping2000Db();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == modifiedBookingDetail.BookingId);
                Guest currentGuest = Db.Guests.SingleOrDefault(i => i.GuestId == currentBooking.GuestId);
                Camping currentSpot = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                Camping modifiedSpot = Db.Camping.SingleOrDefault(i => i.ItemId == modifiedBookingDetail.ItemId);
                if (currentBooking.BookingStartDate != modifiedBookingDetail.BookingStartDate)
                {
                    //handle Change of start Date
                }
                if (currentBooking.BookingEndDate != modifiedBookingDetail.BookingEndDate)
                {
                    //handle Change of end date
                }


                if (currentBooking.BookingNeedsElectricity != modifiedBookingDetail.BookingNeedsElectricity)//Check request of power change
                {
                    if ((currentSpot.CampingElectricity != modifiedBookingDetail.BookingNeedsElectricity) && (modifiedSpot.CampingElectricity != modifiedBookingDetail.BookingNeedsElectricity))
                    {
                        ViewBag.ElectricMessage = "The camping spot choosen do not fullfill the guests wish regarding electric output. ";
                    }
                    else if ((currentSpot.CampingElectricity != modifiedBookingDetail.BookingNeedsElectricity) && (modifiedSpot.CampingElectricity == modifiedBookingDetail.BookingNeedsElectricity))
                    {
                        currentSpot.ItemIsBooked = false;
                        Db.SaveChanges();
                        currentSpot = modifiedSpot;
                        currentSpot.ItemIsBooked = true;
                        Db.SaveChanges();
                        currentBooking.BookingNeedsElectricity = modifiedBookingDetail.BookingNeedsElectricity;
                        currentBooking.ItemId = currentSpot.ItemId;
                        Db.SaveChanges();
                    }
                }

                if (currentBooking.ItemId != modifiedBookingDetail.ItemId)//handle the change of spot
                {
                    if (currentSpot.CampingElectricity == modifiedSpot.CampingElectricity)
                    {
                        currentSpot.ItemIsBooked = false;

                        Db.SaveChanges();
                        currentSpot = modifiedSpot;
                        currentSpot.ItemIsBooked = true;
                        Db.SaveChanges();
                        currentBooking.ItemId = modifiedSpot.ItemId;
                        Db.SaveChanges();
                    }
                    else
                    {
                        currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                        currentSpot.ItemIsBooked = false;
                        currentSpot = modifiedSpot;
                        currentSpot.ItemIsBooked = true;
                        currentBooking.BookingPrice = currentSpot.CampingPrice * currentBooking.NumberOfGuests;
                        currentBooking.BookingNeedsElectricity = modifiedSpot.CampingElectricity;
                        currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;


                    }
                }

                if (currentBooking.NumberOfGuests != modifiedBookingDetail.NumberOfGuests) //Handle number of guest on a spot
                {
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    currentBooking.BookingPrice = currentSpot.CampingPrice * modifiedBookingDetail.NumberOfGuests;
                    currentBooking.NumberOfGuests = modifiedBookingDetail.NumberOfGuests;
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                }
                return PartialView("_UpdatedBooking", modifiedBookingDetail);
            }
            else
            {
                ViewBag.Errormessage = "Guests booking details could not be saved, please try again.";
                return PartialView("_ModifySpecificBooking", modifiedBookingDetail);
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult SearchForGuest(string firstName, string lastName)
        {
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            Camping2000Db Db = new Camping2000Db();
            List<Guest> foundGuests = new List<Guest>();
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
            return PartialView("_ShowFoundGuests", foundGuests);
        }
        [Authorize(Roles = "Administrators")]
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
        [Authorize(Roles = "Administrators")]
        public ActionResult UpdatedGuestDetails([Bind(Include = "GuestFirstName,GuestLastName,GuestNationality,GuestPhoneNumber,GuestMobileNumber," +
            "LivingAdressStreet1,LivingAdressStreet2,LivingAdressStreet3,LivingAdressZipCode,LivingAdressCity," +
            "PostAdressStreet1,PostAdressStreet2,PostAdressStreet3,PostAdressZipCode,PostAdressCity")] GuestAdressViewModel newGuestData)
        {
            Camping2000Db Db = new Camping2000Db();

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
    }
}