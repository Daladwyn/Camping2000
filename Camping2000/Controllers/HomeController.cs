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
        private Camping2000Db Db;

        public HomeController()
        {
            Db = new Camping2000Db();
        }

        [ValidateAntiForgeryToken]
        public ActionResult GoToStart()
        {
            return PartialView("Index");
        }
        public ActionResult Index()
        {
            HttpCookie campingCookie = Request.Cookies["CampingCookie"];
            string cookieBookingId = "";
            Booking ambigiousBooking = new Booking
            {
                BookingId = 0
            };
            if (campingCookie != null)
            {
                cookieBookingId = campingCookie["BookingId"];
                Response.Cookies["CampingCookie"].Expires = DateTime.Now.AddDays(-1);
            }
            if (cookieBookingId != "")
            {
                ambigiousBooking.BookingId = Convert.ToInt32(cookieBookingId);
            }
            return View("Index", ambigiousBooking);
        }
        //flow for making a reservation
        
        //End of reservation flow
        //Start of Checkin flow
       
        //End of Arrival/departures daily flow
        //Start of Editing GuestDeatils flow
        [Authorize(Roles = "Administrators")]
        public ActionResult ModifyGuestDetails()
        {
            return PartialView("_ModifyGuestDetails");
        }
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchForGuest(string firstName, string lastName)
        {
            List<ApplicationUser> foundGuests = new List<ApplicationUser>();
            string errormessage = "";
            if ((firstName == "") && (lastName == ""))//If only empty spaces are supplied as search strings
            {
                ViewBag.Errormessage = "Please specify the guests name before searching.";
                return PartialView("_ShowFoundGuests", foundGuests);
            }
            if ((firstName == null) || (lastName == null))
            {
                ViewBag.Errormessage = "Please specify the guests name before searching(Null).";
                return PartialView("_ShowFoundGuests", foundGuests);
            }
            foundGuests = Camping2000Db.SearchForPeople(firstName, lastName);
            if ((firstName != "") && (foundGuests.Count < 1)) //if first name dont exist as a guests first name
            {
                errormessage = $"A guest with the name of {firstName} was not found. Please try again.";
                ViewBag.Errormessage = errormessage;
            }
            else if ((lastName != "") && (foundGuests.Count < 1))//if last name dont exist as a guests last name
            {
                errormessage = $"A guest with the name of {lastName} was not found. Please try again.";
                ViewBag.Errormessage = errormessage;
            }
            return PartialView("_ShowFoundGuests", foundGuests);
        }
        [Authorize(Roles = "Administrators, Guests")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifySpecificGuestDetails([Bind(Include = "GuestId")]ApplicationUser searchedGuest)
        {
            ApplicationUser foundGuest = Db.Users.SingleOrDefault(i => i.GuestId == searchedGuest.GuestId);
            Adress foundAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == searchedGuest.GuestId);
            if ((foundGuest == null) || (foundAdress == null)) //if no data was fetched, alert guest
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_ModifyGuestDetails");
            }
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
        [HttpPost]
        [Authorize(Roles = "Administrators, Guests")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatedGuestDetails([Bind(Include = "GuestFirstName,GuestLastName,GuestNationality,GuestPhoneNumber,GuestMobileNumber," +
                            "GuestId,LivingAdressStreet1,LivingAdressStreet2,LivingAdressStreet3,LivingAdressZipCode,LivingAdressCity," +
                            "PostAdressStreet1,PostAdressStreet2,PostAdressStreet3,PostAdressZipCode,PostAdressCity")] GuestAdressViewModel newGuestData)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser oldGuestData = Db.Users.SingleOrDefault(i => i.GuestId == newGuestData.GuestId);
                Adress oldGuestAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == newGuestData.GuestId);
                if ((oldGuestData == null) || (oldGuestAdress == null))
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return PartialView("_ModifyGuestDetails");
                }
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
            else
            {
                ViewBag.Errormessage = "Some of the supplied values were incorrect. Please try again, with other possible values.";
                return PartialView("_GuestDetails", newGuestData);
            }
        }
        //End of Editing GuestDeatils flow
        //Start of modifying Booking flow
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult ModifyBooking()
        {
            List<Booking> allBookings = Db.Bookings.ToList();
            List<BookingGuestViewModel> presentGuestBookings = new List<BookingGuestViewModel>();
            List<Booking> presentBookings = new List<Booking>();
            List<ApplicationUser> presentGuests = new List<ApplicationUser>();
            List<Camping> presentSpots = new List<Camping>();
            ViewBag.Errormessage = "";
            if (allBookings == null)
            {
                presentGuestBookings = null;
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_ModifyBooking", presentGuestBookings);
            }
            for (int i = 0; i < allBookings.Count; i++)
            {
                if (((allBookings[i].GuestHasReserved == true) || (allBookings[i].GuestHasCheckedIn == true)) && (allBookings[i].BookingIsPaid == false))
                {
                    presentBookings.Add(allBookings[i]);
                }
            }
            if (presentBookings.Count < 1)
            {
                presentGuestBookings = null;
                ViewBag.Errormessage = "No bookings are available to modify.";
                return PartialView("_ModifyBooking", presentGuestBookings);
            }
            foreach (var booking in presentBookings)
            {
                presentGuests.Add(Db.Users.SingleOrDefault(i => i.Id == booking.GuestId));
                presentSpots.Add(Db.Camping.SingleOrDefault(i => i.ItemId == booking.ItemId));
            }
            if ((presentBookings.Count != presentGuests.Count) && (presentBookings.Count != presentGuests.Count))
            {
                ViewBag.Errormessage = "Preparation of data did not succeed. Please try again.";
                presentGuestBookings = null;
                return PartialView("_ModifyBooking", presentGuestBookings);
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
        [Authorize(Roles = "Administrators, Receptionists")]
        public ActionResult ListPresentBookings(BookingGuestViewModel AGuestBooking)
        {
            return PartialView("_PresentBooking", AGuestBooking);
        }
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifySpecificBooking([Bind(Include = "BookingId,ItemId,GuestId")] ModifyBookingViewModel aBookingToModify)
        {
            if (ModelState.IsValid)
            {
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == aBookingToModify.BookingId);
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == currentBooking.GuestId);
                Camping currentSpot = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                List<Camping> allSpots = Db.Camping.ToList();
                List<Camping> freeSpots = new List<Camping>();
                if ((currentBooking == null) || (currentGuest == null) || (currentSpot == null) || (allSpots == null))
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return RedirectToAction("ModifySpecificBooking", aBookingToModify);
                }
                foreach (var spot in allSpots)
                {
                    if ((spot.ItemIsOccupied == false) && (spot.CampingElectricity == currentBooking.BookingNeedsElectricity))
                    {
                        freeSpots.Add(spot);
                    }
                }
                //if (freeSpots.Count < 1)
                //{
                //    freeSpots = null;
                //}
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
                ViewBag.Errormessage = "Some of the submitted values were incorrect. Please try again.";
                return PartialView("_ModifySpecificBooking", aBookingToModify);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStartDate([Bind(Include = "BookingId,GuestId,ItemId,BookingStartDate")] ModifyBookingViewModel bookingToModify)
        {
            ModifyBookingViewModel currentBookingView = new ModifyBookingViewModel();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> bookingsWSCSpot = new List<Booking>(); //bookingsWithSameCampingpot 
            List<Booking> bookingsWSCSECB = new List<Booking>(); //bookingsWithSameCampingpotExcludingCurrentBooking 

            List<Booking> bookingsWithSameCampingpot = new List<Booking>();
            List<int> disAllowableBookings = new List<int>();
            List<Camping> ListOfSpots = new List<Camping>();
            int numberOfDays = 0;
            if ((currentBooking == null) || (currentGuest == null) || (currentItem == null) || (allBookings == null))
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_FailedChangeStartDate", bookingToModify);
            }
            if (bookingToModify.BookingStartDate >= currentBooking.BookingEndDate)
            {
                ViewBag.Errormessage = "Start date cannot be after end date. Please choose another start date.";
                return PartialView("_FailedChangeStartDate", bookingToModify);
            }
            foreach (var booking in allBookings) //gather all bookings that have the same Itemid as the present one 
            {
                if ((booking.ItemId == currentItem.ItemId) && (booking.BookingEndDate > bookingToModify.BookingStartDate) && (booking.BookingStartDate < bookingToModify.BookingStartDate))
                {
                    bookingsWithSameCampingpot.Add(booking);
                }
            }
            foreach (var booking in bookingsWithSameCampingpot)
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
                numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
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
                ListOfSpots = Booking.FetchCampingSpots(currentBooking.BookingNeedsElectricity);
                if (ListOfSpots[0].CampingSpot == "NoData")
                {
                    ViewBag.Errormessage = "Fetching campingdata did not succeed. Please try again.";
                    return PartialView("_FailedChangeStartDate", bookingToModify);
                }
                ListOfSpots = Camping.RemoveOccupiedSpots(ListOfSpots, disAllowableBookings);

                if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                {
                    ViewBag.Errormessage = "There are no available space for you. Please choose another arrivaldate.";
                    return PartialView("_FailedChangeStartDate", bookingToModify);
                }
                currentBooking.ItemId = ListOfSpots[0].ItemId;
                currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                //Calculate the price for the guest
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingStartDate = bookingToModify.BookingStartDate;
                numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = currentItem.CampingPrice * numberOfDays * currentBooking.NumberOfGuests;
                Db.SaveChanges();
                bookingToModify.BookingStartDate = currentBooking.BookingStartDate;
                bookingToModify.BookingEndDate = currentBooking.BookingEndDate;
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.NumberOfGuests = currentBooking.NumberOfGuests;
                return PartialView("_ChangeStartDate", bookingToModify);
            }
        }
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEndDate([Bind(Include = "BookingId,GuestId,ItemId,BookingEndDate")] ModifyBookingViewModel bookingToModify)
        {
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> bookingsThatCollide = new List<Booking>();
            List<int> disAllowableBookings = new List<int>();
            List<Camping> allSpots = Db.Camping.ToList();
            List<Camping> ListOfSpots = new List<Camping>();
            int numberOfDays = 0;
            if ((currentBooking == null) || (currentGuest == null) || (currentItem == null) || (allBookings == null))
            {
                ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                return PartialView("_FailedChangeEndDate", bookingToModify);
            }
            if (bookingToModify.BookingEndDate <= currentBooking.BookingStartDate)
            {
                ViewBag.Errormessage = "End date cannot be before start date. Please choose another end date.";
                return PartialView("_FailedChangeEndDate", bookingToModify);
            }
            foreach (var booking in allBookings)//check for bookings at same spot that start before the new end data 
            {
                if ((booking.ItemId == currentItem.ItemId) && ((booking.BookingStartDate < bookingToModify.BookingEndDate) && (booking.BookingStartDate > currentBooking.BookingEndDate)))
                {
                    bookingsThatCollide.Add(booking);
                }
            }
            if (bookingsThatCollide.Count < 1)//If no other bookings start before new enddata, change endday and booking price
            {
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingEndDate = bookingToModify.BookingEndDate;
                numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                Db.SaveChanges();
                ViewBag.Message = "The rescheduling of the enddate succeded with no change in placement.";
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.BookingStartDate = currentBooking.BookingStartDate;
                bookingToModify.ItemName = currentItem.ItemName;
                return PartialView("_ChangeEndDate", bookingToModify);
            }
            else
            {
                ViewBag.Message = "A change of spot is needed.";
                foreach (var booking in allBookings) //gather data of occupied spaces
                {
                    if (((booking.GuestHasCheckedIn == true) || (booking.GuestHasReserved == true)) && ((booking.BookingEndDate > currentBooking.BookingStartDate) || (booking.BookingStartDate < currentBooking.BookingEndDate)))
                    {
                        disAllowableBookings.Add(booking.ItemId);
                    }
                }
                disAllowableBookings.Sort();
                ListOfSpots = Booking.FetchCampingSpots(currentBooking.BookingNeedsElectricity);//Fetch Campingpots based on power needs
                if (ListOfSpots[0].CampingSpot == "NoData")
                {
                    ViewBag.Errormessage = "Fetching campingdata did not succeed. Please try again.";
                    return PartialView("_FailedChangeEndDate");
                }
                ListOfSpots = Camping.RemoveOccupiedSpots(ListOfSpots, disAllowableBookings); //remove spots that are already occupied

                if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                {
                    ViewBag.Errormessage = "No spots are available for the new end date. Please choose another departuredate.";
                    return PartialView("_FailedChangeEndDate");
                }
                currentBooking.ItemId = ListOfSpots[0].ItemId;//make the swith to new spot
                currentItem.ItemIsOccupied = false;
                currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
                currentItem.ItemIsOccupied = true;
                //Calculate the price for the guest
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                currentBooking.BookingEndDate = bookingToModify.BookingEndDate;
                numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                currentBooking.BookingPrice = currentItem.CampingPrice * numberOfDays * currentBooking.NumberOfGuests;
                currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                Db.SaveChanges();
                bookingToModify.BookingPrice = currentBooking.BookingPrice;
                bookingToModify.BookingStartDate = currentBooking.BookingStartDate;
                bookingToModify.ItemName = currentItem.ItemName;
                return PartialView("_ChangeEndDate", bookingToModify);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePowerOutlet([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                List<ModifyBookingViewModel> currentBookingView = new List<ModifyBookingViewModel>();
                ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
                ModifyBookingViewModel anotherBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Booking> allBookings = Db.Bookings.ToList();
                List<int> disAllowableBookings = new List<int>();
                List<Booking> bookingsWithSameCampingpot = new List<Booking>();
                List<Camping> ListOfSpots = new List<Camping>();
                int numberOfDays = 0;
                List<Booking> lb = new List<Booking>();
                LinkBooking linkedBooking = new LinkBooking();
                if ((currentBooking == null) || (currentGuest == null) || (currentItem == null) || (allBookings == null))//validate the data to be legit
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return PartialView("_FailedChangePowerOutlet");//, currentBooking
                }
                if (currentBooking.GuestHasCheckedIn == false) //If guest have not checked in, then no new booking is necesary
                {
                    currentBooking.BookingNeedsElectricity = (currentBooking.BookingNeedsElectricity == false) ? currentBooking.BookingNeedsElectricity = true : currentBooking.BookingNeedsElectricity = false; //switch the electrical needs of the booking.
                    foreach (var booking in Db.Bookings)//see if any bookings exits that collide with the current booking
                    {
                        if ((booking.BookingNeedsElectricity == currentBooking.BookingNeedsElectricity) && (((booking.BookingEndDate > currentBooking.BookingStartDate) && (booking.BookingEndDate <= currentBooking.BookingEndDate)) || ((booking.BookingStartDate < currentBooking.BookingEndDate) && (booking.BookingStartDate >= currentBooking.BookingStartDate))))
                        {
                            disAllowableBookings.Add(booking.ItemId); //gather all bookings that have dates that collide 
                        }
                    }
                    disAllowableBookings.Sort();
                    ListOfSpots = Booking.FetchCampingSpots(currentBooking.BookingNeedsElectricity); //get all spots based on power needs
                    if (ListOfSpots[0].CampingSpot == "NoData")
                    {
                        ViewBag.Errormessage = "Fetching campingdata did not succeed. Please try again.";
                        return PartialView("_FailedChangePowerOutlet");//, currentBooking
                    }
                    if (disAllowableBookings.Count < 1)//if no collision is detected and the guest is not checked in, change spot and calculate new cost
                    {
                        currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                        numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate); ;
                        currentItem = ListOfSpots[0];
                        currentBooking.ItemId = currentItem.ItemId;
                        currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                        Db.SaveChanges();
                        //a controll of the save data should be here.
                        lb.Add(currentBooking);
                        ViewBag.Message = "The change of poweroutlet succeded. See details below.";
                        return PartialView("_ChangePowerOutlet", lb);
                    }
                    ListOfSpots = Camping.RemoveOccupiedSpots(ListOfSpots, disAllowableBookings); //remove spots that have colliding bookings

                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There are no available spots that matches your need for electricity.";
                        return PartialView("_FailedChangePowerOutlet");//, currentBooking
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice; //remove the present bookingprice
                    numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    currentItem.ItemIsOccupied = false; //release the "old" spot
                    currentBooking.ItemId = ListOfSpots[0].ItemId; //assign the new spot
                    currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId); //fetch the new spots data
                    if (currentItem == null)
                    {
                        ViewBag.Errormessage = "Fetching data of new spot did not succed.";
                        return PartialView("_FailedChangePowerOutlet");//, currentBooking
                    }
                    currentItem.ItemIsOccupied = true;
                    currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;//Calculate the new price
                    Db.SaveChanges();
                    //a controll of the save data should be here.
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate; //prepare data for the view
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
                    bookingToModify.BookingEndDate = currentBooking.BookingEndDate;//transfer values to a new instance of the variable
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
                    //a controll of save data should be here
                    newBooking.BookingNeedsElectricity = bookingToModify.BookingNeedsElectricity;
                    newBooking.BookingPrice = 0;
                    newBooking.GuestId = bookingToModify.GuestId;
                    Db.SaveChanges();
                    //a controll of save data should be here
                    foreach (var booking in allBookings)//see if any bookings exits that collide with the current booking
                    {
                        if ((booking.BookingNeedsElectricity == currentBooking.BookingNeedsElectricity) && ((booking.BookingEndDate > currentBooking.BookingStartDate) || (booking.BookingStartDate < currentBooking.BookingEndDate)))
                        {
                            disAllowableBookings.Add(booking.ItemId);
                        }
                    }
                    disAllowableBookings.Sort();
                    ListOfSpots = Booking.FetchCampingSpots(bookingToModify.BookingNeedsElectricity); //fetch spots based on powerneeds
                    if (ListOfSpots[0].CampingSpot == "NoData")
                    {
                        ViewBag.Errormessage = "Fetching campingdata did not succeed. Please try again.";
                        return PartialView("_FailedChangePowerOutlet");//, currentBooking
                    }
                    ListOfSpots = Camping.RemoveOccupiedSpots(ListOfSpots, disAllowableBookings); //remove the spots that have colliding bookings

                    if (ListOfSpots.Count == 0) //if no spots remains send a message to user that camping is full
                    {
                        ViewBag.Errormessage = "There are no available spots that matches your need for electricity.";
                        return PartialView("_FailedChangePowerOutlet");//, lb
                    }
                    newBooking.ItemId = ListOfSpots[0].ItemId;
                    currentItem.ItemIsOccupied = false;
                    currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId); //fetch the new spots data
                    if (currentItem == null)
                    {
                        ViewBag.Errormessage = "Fetching data of new spot did not succed.";
                        return PartialView("_FailedChangePowerOutlet");//, currentBooking
                    }
                    currentItem.ItemIsOccupied = true;
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
                    if (numberOfDays == 0)
                    {
                        ViewBag.Errormessage = "The change of poweroutlet is on the same day as checkin day.";
                        currentBooking.BookingPrice = 0;
                        currentBooking.BookingIsPaid = true;
                        Db.SaveChanges();
                        //may need to redirect to 
                    }
                    else
                    {
                        currentBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * currentBooking.NumberOfGuests;
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice;
                    Db.SaveChanges();
                    //Add a controll that checks saved data
                    aBookingView.BookingEndDate = currentBooking.BookingEndDate; //prepare data to the view
                    aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    aBookingView.ItemName = currentItem.ItemName;
                    aBookingView.BookingPrice = currentBooking.BookingPrice;
                    aBookingView.BookingId = currentBooking.BookingId;
                    aBookingView.GuestId = currentBooking.GuestId;
                    aBookingView.ItemId = currentItem.ItemId;
                    aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    currentBookingView.Add(aBookingView);
                    numberOfDays = Booking.CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate);
                    if (numberOfDays == 0)
                    {
                        ViewBag.Errormessage = "The change of poweroutlet is on the same day as checkout day.";
                        newBooking.BookingPrice = 0;
                        newBooking.BookingIsPaid = true;
                    }
                    else
                    {
                        newBooking.BookingPrice = numberOfDays * currentItem.CampingPrice * newBooking.NumberOfGuests;
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + newBooking.BookingPrice;
                    Db.SaveChanges();
                    //a controller to check saved data
                    anotherBookingView.BookingEndDate = newBooking.BookingEndDate;//prepare the data for the view
                    anotherBookingView.BookingStartDate = newBooking.BookingStartDate;
                    anotherBookingView.ItemName = currentItem.ItemName;
                    anotherBookingView.BookingPrice = newBooking.BookingPrice;
                    anotherBookingView.BookingId = newBooking.BookingId;
                    anotherBookingView.GuestId = newBooking.GuestId;
                    anotherBookingView.ItemId = currentItem.ItemId;
                    anotherBookingView.NumberOfGuests = newBooking.NumberOfGuests;
                    anotherBookingView.BookingNeedsElectricity = newBooking.BookingNeedsElectricity;
                    currentBookingView.Add(anotherBookingView);
                    linkedBooking.PreBooking = currentBooking.BookingId;
                    linkedBooking.PostBooking = newBooking.BookingId;
                    Db.LinkBookings.Add(linkedBooking);
                    if (Db.SaveChanges() != 1)
                    {
                        string message = "The link between booking " + currentBooking.BookingId + " and booking " + newBooking.BookingId + " could not be saved.";
                        ViewBag.Errormessage = message;
                        return PartialView("_FailedChangePowerOutlet");
                    }

                    ViewBag.Message = "The change of poweroutlet succeded. See details below.";
                    return PartialView("_ChangePowerOutlet", currentBookingView);
                }
            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePartySize([Bind(Include = "BookingId,GuestId,ItemId,NumberOfGuests")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                List<ModifyBookingViewModel> currentBookingView = new List<ModifyBookingViewModel>();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Booking> lb = new List<Booking>();
                LinkBooking linkedBooking = new LinkBooking();
                int numberOfDays = 0;
                if ((currentBooking == null) || (currentGuest == null) || (currentItem == null))
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    lb = null;
                    return PartialView("_ChangePartySize", lb);
                }
                if (currentBooking.GuestHasCheckedIn == false)
                {
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice;
                    numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate);
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
                    //add a controll of saved data
                    bookingToModify.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    newBooking.BookingNeedsElectricity = bookingToModify.BookingNeedsElectricity;
                    newBooking.BookingPrice = 0;
                    newBooking.GuestId = bookingToModify.GuestId;
                    newBooking.ItemId = bookingToModify.ItemId;
                    Db.SaveChanges();
                    //add a controll of saved data
                    currentBooking.BookingEndDate = DateTime.Now;   //change end date for present booking
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay - currentBooking.BookingPrice; //subtract the present bookingprice for the guest to pay 
                    numberOfDays = Booking.CalculateNumberOfDays(currentBooking.BookingStartDate, currentBooking.BookingEndDate); //calculate number of days the present stay was
                    if (numberOfDays == 0)
                    {
                        currentBooking.BookingPrice = 0; //if guest changes number of persons before first night but after checkin
                    }
                    else
                    {
                        currentBooking.BookingPrice = numberOfDays * currentBooking.NumberOfGuests * currentItem.CampingPrice; //calculate the present bookingprce
                    }
                    lb.Add(currentBooking);
                    numberOfDays = Booking.CalculateNumberOfDays(newBooking.BookingStartDate, newBooking.BookingEndDate);  //calculate the days in the new booking
                    if (numberOfDays != 0)
                    {
                        newBooking.BookingPrice = numberOfDays * newBooking.NumberOfGuests * currentItem.CampingPrice;   //calculate the new bookings price
                    }
                    else
                    {
                        ViewBag.Errormessage = "Guest can not change partysize last booked day.";
                        //return PartialView("_ChangePartySize");
                    }
                    currentGuest.GuestHasToPay = currentGuest.GuestHasToPay + currentBooking.BookingPrice + newBooking.BookingPrice; //update the amount the guest have to pay 
                    lb.Add(newBooking);
                    Db.Bookings.Add(newBooking);// is this neccesary?
                    Db.SaveChanges();
                    //Adding controll to check saved data
                    linkedBooking.PreBooking = currentBooking.BookingId;
                    linkedBooking.PostBooking = newBooking.BookingId;
                    Db.LinkBookings.Add(linkedBooking);
                    if (Db.SaveChanges() != 1)
                    {
                        string message = "The link between booking " + currentBooking.BookingId + " and booking " + newBooking.BookingId + " could not be saved.";
                        ViewBag.Errormessage = message;
                        return PartialView("_ChangePartySize");
                    }
                    return PartialView("_ChangePartySize", lb);
                }
            }
            else
            {
                if (bookingToModify.NumberOfGuests < 1)
                {
                    ViewBag.Errormessage = "Number of guests is less than 0. Number of guests has to be between 1 and 10.";
                }
                if (bookingToModify.NumberOfGuests > 10)
                {
                    ViewBag.Errormessage = "Number of guests is greater than 10. Number of guests has to be between 1 and 10.";
                }
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCampingSpot([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Camping> allSpots = Db.Camping.ToList();
                List<Booking> allBookings = Db.Bookings.ToList();
                List<Camping> availableSpots = new List<Camping>();
                List<Booking> collidingBookings = new List<Booking>();
                List<Booking> collidingBookingsWPN = new List<Booking>();//collidingBookingsWithPowerNeed
                if ((currentBooking == null) || (currentGuest == null) || (currentItem == null) || (allSpots == null) || (allBookings == null))
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return PartialView("_ChangeCampingSpot");
                }
                if (currentBooking.GuestHasCheckedIn == true)
                {
                    foreach (var spot in allSpots)
                    {
                        if ((spot.ItemIsOccupied == false) && (spot.ItemId != currentItem.ItemId) && (spot.CampingElectricity == currentBooking.BookingNeedsElectricity))
                        {
                            availableSpots.Add(spot);
                        }
                    }
                    if (availableSpots.Count == 0)
                    {
                        ViewBag.Errormessage = "No spots are vacant, preventing a change of spot.";
                        return PartialView("_ChangeCampingSpot");
                    }
                    foreach (var booking in allBookings) //detect colliding bookings
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
                    //aBookingView.ItemId = currentItem.ItemId;
                    //aBookingView.ItemName = currentItem.ItemName;
                    //aBookingView.BookingId = currentBooking.BookingId;
                    //aBookingView.GuestId = currentGuest.GuestId;
                    //aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                    //aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                    //aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                    //aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                    aBookingView.VacantSpots = availableSpots;
                    //return PartialView("_ChangeCampingpot", aBookingView);
                }
                else
                {
                    //inform that change of spot is only doable after checkin.
                    ViewBag.GuestHasCheckedIn = "false";
                }
                aBookingView.ItemId = currentItem.ItemId;
                aBookingView.ItemName = currentItem.ItemName;
                aBookingView.BookingId = currentBooking.BookingId;
                aBookingView.GuestId = currentGuest.GuestId;
                aBookingView.BookingStartDate = currentBooking.BookingStartDate;
                aBookingView.BookingEndDate = currentBooking.BookingEndDate;
                aBookingView.NumberOfGuests = currentBooking.NumberOfGuests;
                aBookingView.BookingNeedsElectricity = currentBooking.BookingNeedsElectricity;
                return PartialView("_ChangeCampingSpot", aBookingView);

            }
            else
            {
                return RedirectToAction("ModifySpecificBooking", bookingToModify);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeChooseCampingSpot([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            ModifyBookingViewModel aBookingView = new ModifyBookingViewModel();
            Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
            ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
            Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
            Camping oldItem = Db.Camping.SingleOrDefault(i => i.ItemId == currentBooking.ItemId);
            if (ModelState.IsValid)
            {
                if ((currentBooking == null) || (currentGuest == null) || (currentItem == null) || (oldItem == null))
                {
                    ViewBag.Errormessage = "Fetching data did not succeed. Please try again.";
                    return PartialView("_ChangeConfirmationCampingpot");
                }
                oldItem.ItemIsOccupied = false;
                Db.SaveChanges();
                //add controll of saved data
                currentItem.ItemIsOccupied = true;
                currentBooking.ItemId = currentItem.ItemId;
                Db.SaveChanges();
                //add controll of saved data
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
        [HttpPost]
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReservation([Bind(Include = "BookingId,GuestId,ItemId")] ModifyBookingViewModel bookingToModify)
        {
            if (ModelState.IsValid)
            {
                ModifyBookingViewModel currentBookingView = new ModifyBookingViewModel();
                Booking currentBooking = Db.Bookings.SingleOrDefault(i => i.BookingId == bookingToModify.BookingId);
                if (currentBooking == null)
                {
                    ViewBag.Errormessage = "The booking is already cancelled.";
                    return PartialView("_FailedCancelReservation", bookingToModify);
                }
                ApplicationUser currentGuest = Db.Users.SingleOrDefault(i => i.GuestId == bookingToModify.GuestId);
                Camping currentItem = Db.Camping.SingleOrDefault(i => i.ItemId == bookingToModify.ItemId);
                List<Booking> guestsOtherBookings = new List<Booking>();
                List<Booking> allbookings = Db.Bookings.ToList();
                foreach (var booking in allbookings)
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
        //End of modifying Booking flow
        //Start of guest registration flow
        [AllowAnonymous]
        public ActionResult GuestData(ApplicationUser user)
        {
            GuestDataViewModel newGuest = new GuestDataViewModel()
            {
                GuestId = user.Id,
                EmailAddress = user.Email
            };
            return PartialView("_GuestData", newGuest);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveGuestData([Bind(Include = "GuestFirstName,GuestLastName,GuestNationality,GuestPhoneNumber,GuestMobileNumber," +
                            "GuestId,EmailAddress,PostAdressStreet1,PostAdressStreet2,PostAdressStreet3,PostAdressZipCode,PostAdressCity," +
                            "LivingAdressStreet1,LivingAdressStreet2,LivingAdressStreet3,LivingAdressZipCode,LivingAdressCity")]GuestDataViewModel newGuest)
        {
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<ApplicationUser>(Db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                Booking guestBooking = new Booking();
                HttpCookie campingCookie = Request.Cookies["CampingCookie"];
                string cookieBookingId = "";
                Response.Cookies["CampingCookie"].Expires = DateTime.Now.AddDays(-1);
                ApplicationUser user = Db.Users.SingleOrDefault(i => i.Id == newGuest.GuestId);
                user.Id = newGuest.GuestId;
                user.GuestId = newGuest.GuestId;
                user.GuestFirstName = newGuest.GuestFirstName;
                user.GuestLastName = newGuest.GuestLastName;
                user.GuestNationality = newGuest.GuestNationality;
                user.GuestHasCheckedIn = false;
                user.GuestHasPaid = 0;
                user.GuestHasReserved = false;
                user.GuestHasToPay = 0;
                user.GuestMobileNumber = newGuest.GuestMobileNumber;
                user.GuestPhoneNumber = newGuest.GuestPhoneNumber;
                int num1 = Db.SaveChanges();
                Adress guestAdress = new Adress()
                {
                    GuestId = newGuest.GuestId,
                    LivingAdressCity = newGuest.LivingAdressCity,
                    LivingAdressStreet1 = newGuest.LivingAdressStreet1,
                    LivingAdressStreet2 = newGuest.LivingAdressStreet2,
                    LivingAdressStreet3 = newGuest.LivingAdressStreet3,
                    LivingAdressZipCode = newGuest.LivingAdressZipCode,
                    PostAdressCity = newGuest.PostAdressCity,
                    PostAdressStreet1 = newGuest.PostAdressStreet1,
                    PostAdressStreet2 = newGuest.PostAdressStreet2,
                    PostAdressStreet3 = newGuest.PostAdressStreet3,
                    PostAdressZipCode = newGuest.PostAdressZipCode
                };
                Db.Adresses.Add(guestAdress);
                if (Db.SaveChanges() != 1)
                {
                    ViewBag.Errormessage = "Saving your adressdata did not succed. Please contact the campingstaff.";
                    return PartialView("_GuestData", newGuest);
                }
                userManager.AddToRole(user.Id, "Guests");//add the guest to the role of "Guests"
                if (Db.SaveChanges() != 0)
                {
                    ViewBag.Errormessage = "Giving guest role as a guest did not succed. Please contact the campingstaff.";
                    return PartialView("_GuestData", newGuest);
                };
                if (campingCookie != null)
                {
                    cookieBookingId = campingCookie["BookingId"];
                    Response.Cookies["CampingCookie"].Expires = DateTime.Now.AddDays(-1);
                    if (cookieBookingId != "")
                    {
                        guestBooking.BookingId = Convert.ToInt32(cookieBookingId);
                        guestBooking.GuestId = newGuest.GuestId;
                    }
                    int num4 = Db.SaveChanges();
                    if (Db.SaveChanges() != 0)
                    {
                        ViewBag.Errormessage = "Saving your incomplete reservation did not succed. Please contact the campingstaff.";
                        return PartialView("_GuestData", newGuest);
                    }
                }
                return PartialView("_RegistrationComplete", guestBooking);
            }
            else
            {
                ViewBag.Errormessage = "Some of your submitted values were not correct. Please try again.";
                return PartialView("_GuestData", newGuest);
            }
        }
        //End of guest registration flow
        //Start of modify guest data flow
        [Authorize(Roles = "Administrators, Receptionists, Guests")]
        public ActionResult GuestDetails(string GuestId)
        {
            ApplicationUser foundGuest = Db.Users.SingleOrDefault(i => i.GuestId == GuestId);
            if (foundGuest == null)
            {
                ViewBag.Errormessage = "No user data was found. Are you the admin?";
                return PartialView("_FailedGuestDetails");
            }
            else
            {
                Adress foundAdress = Db.Adresses.SingleOrDefault(i => i.GuestId == GuestId);
                if (foundAdress == null)
                {
                    ViewBag.Errormessage = "No adress was fetchable. Please try again.";
                    return PartialView("_FailedGuestDetails");
                }
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
        //End of modify guest data flow

        //Start of bookinglist that turns up forgotten bookings 
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult MissedCheckins()
        {
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> failedCheckins = new List<Booking>();
            ApplicationUser guestThatFailedToCheckin = new ApplicationUser();
            GuestBookingViewModel failedBooking = new GuestBookingViewModel();
            List<GuestBookingViewModel> allFailedBookings = new List<GuestBookingViewModel>();
            if (allBookings == null)
            {
                ViewBag.Errormessage = "Fetching data did not succed. Please try again.";
                return PartialView("_MissedCheckins");
            }
            foreach (var booking in allBookings)//fetch the bookings that have not been checked in
            {
                if ((booking.GuestHasReserved == true) && (booking.BookingStartDate < DateTime.Now.Date))
                {
                    failedCheckins.Add(booking);
                }
            }
            foreach (var booking in failedCheckins)//populate the view with data from list of failedcheckins
            {
                guestThatFailedToCheckin = Db.Users.SingleOrDefault(g => g.GuestId == booking.GuestId);
                failedBooking.BookingId = booking.BookingId;
                failedBooking.BookingStartDate = booking.BookingStartDate;
                failedBooking.BookingEndDate = booking.BookingEndDate;
                failedBooking.NumberOfGuests = booking.NumberOfGuests;
                failedBooking.GuestFirstName = guestThatFailedToCheckin.GuestFirstName;
                failedBooking.GuestLastName = guestThatFailedToCheckin.GuestLastName;
                failedBooking.GuestId = guestThatFailedToCheckin.GuestId;
                failedBooking.GuestMobileNumber = guestThatFailedToCheckin.GuestMobileNumber;
                failedBooking.GuestPhoneNumber = guestThatFailedToCheckin.GuestPhoneNumber;
                failedBooking.ItemId = booking.ItemId;
                allFailedBookings.Add(failedBooking);
            }
            return PartialView("_MissedCheckins", allFailedBookings);
        }
        [HttpPost]
        [Authorize(Roles = "Administrators, Receptionists")]
        [ValidateAntiForgeryToken]
        public ActionResult MissedCheckouts()
        {
            List<Booking> allBookings = Db.Bookings.ToList();
            List<Booking> failedCheckouts = new List<Booking>();
            ApplicationUser guestThatFailedToCheckOut = new ApplicationUser();
            GuestBookingViewModel failedBooking = new GuestBookingViewModel();
            List<GuestBookingViewModel> allFailedBookings = new List<GuestBookingViewModel>();
            if (allBookings == null)
            {
                ViewBag.Errormessage = "Fetching data did not succed. Please try again.";
                return PartialView("_MissedCheckOuts");
            }
            foreach (var booking in allBookings)//fetch the bookings that have not been checked in
            {
                if ((booking.GuestHasCheckedIn == true) && (booking.BookingEndDate < DateTime.Now.Date))
                {
                    failedCheckouts.Add(booking);
                }
            }
            foreach (var booking in failedCheckouts)//populate the view with data from list of failedcheckins
            {
                guestThatFailedToCheckOut = Db.Users.SingleOrDefault(g => g.GuestId == booking.GuestId);
                if (guestThatFailedToCheckOut != null)
                {
                    failedBooking.BookingId = booking.BookingId;
                    failedBooking.BookingStartDate = booking.BookingStartDate;
                    failedBooking.BookingEndDate = booking.BookingEndDate;
                    failedBooking.NumberOfGuests = booking.NumberOfGuests;
                    failedBooking.GuestFirstName = guestThatFailedToCheckOut.GuestFirstName;
                    failedBooking.GuestLastName = guestThatFailedToCheckOut.GuestLastName;
                    failedBooking.GuestId = guestThatFailedToCheckOut.GuestId;
                    failedBooking.GuestMobileNumber = guestThatFailedToCheckOut.GuestMobileNumber;
                    failedBooking.GuestPhoneNumber = guestThatFailedToCheckOut.GuestPhoneNumber;
                    failedBooking.ItemId = booking.ItemId;
                    failedBooking.BookingNeedsElectricity = booking.BookingNeedsElectricity;
                    failedBooking.BookingPrice = booking.BookingPrice;
                    allFailedBookings.Add(failedBooking);
                }
            }
            return PartialView("_MissedCheckOuts", allFailedBookings);
        }
        //End of bookinglist that turns up forgotten bookings

        //        [HttpPost]
        //        public ActionResult ShowVacantFullsign()
        //{
        //    return PartialView("ShowVacantFullSign");
        //}
        //[Authorize(Roles = "Administrators")]
        //public ActionResult ShowVacantSpots()
        //{
        //    List<Camping> vacantSpots = new List<Camping>();
        //    using (var context = new Camping2000Db())
        //    {
        //        foreach (var spot in context.Camping)
        //        {
        //            if (spot.ItemIsOccupied == false)
        //            {
        //                vacantSpots.Add(spot);
        //            }
        //        }
        //    }
        //    if (vacantSpots == null)
        //    {
        //        return PartialView("_NoAvailableSpotToChangeTo");
        //    }
        //    return PartialView("_ShowVacantSpots", vacantSpots);
        //}
        //public ActionResult Logoff()
        //{
        //    return RedirectToAction("Logoff", "Account");
        //}

       
        
       
       
       


    }
}