using Camping2000.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Camping2000.Controllers
{
    public class ReceptionistController : Controller
    {
        private Camping2000Db Db;

        public ReceptionistController()
        {
            Db = new Camping2000Db();
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult ManageReceptionists()
        {
            return PartialView("_ManageReceptionists");
        }

        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchNewReceptionist(string firstName, string lastName)
        {
            List<ApplicationUser> foundGuests = new List<ApplicationUser>();
            string errormessage = "";
            if ((firstName == "") && (lastName == ""))//Check typed string to be something else than void
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
            return PartialView("_ShowFoundCoworker", foundGuests);
        }

        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyCoworkerToReceptionist(string GuestId)
        {
            //Camping2000Db Db = new Camping2000Db();
            var userStore = new UserStore<ApplicationUser>(Db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            Receptionist newCoWorker = new Receptionist//Create a new coworker and transfer guestId 
            {
                GuestId = GuestId
            };
            Receptionist CoWorker = Db.Receptionists.SingleOrDefault(g => g.GuestId == GuestId);//try and fetch the guestid 
            if (CoWorker != null) //if coworker already exists in receptionist he should not be able to be granted rights
            {
                ViewBag.RightsMessage = "The new coworker have already rights as a receptionist";
                return PartialView("_ConfirmReceptionistRights");
            }
            Db.Receptionists.Add(newCoWorker); //Add the new coworker to receptionists list
            if (Db.SaveChanges() != 1)
            {
                ViewBag.RightsMessage = "Saving data did not succed. Please try again.";
                return PartialView("_ConfirmReceptionistRights");
            };
            var user = userManager.FindById(GuestId); //find the new coWorker by Id
            if (user == null)
            {
                ViewBag.RightsMessage = "User have to exist in 2 userdatabases, but are currently not doing so.";
                return PartialView("_ConfirmReceptionistRights");
            }
            userManager.AddToRole(user.Id, "Receptionists");//add new coWorker to the role of "Receptionists"
            userManager.RemoveFromRole(user.Id, "Guests");//Remove the role of "Guest" from new coworker 
            if (Db.SaveChanges() != 0)
            {
                ViewBag.RightsMessage = "Saving data did not succed. Please try again.";
                return PartialView("_ConfirmReceptionistRights");
            }
            ViewBag.RightsMessage = "The new coworker have now rights as a receptionist";
            return PartialView("_ConfirmReceptionistRights");
        }

        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyCoWorkerToGuest(string GuestId)
        {
            //Save the removal of old coworkers ID in the receptionists table
            var userStore = new UserStore<ApplicationUser>(Db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            List<Receptionist> allReceptionists = Db.Receptionists.ToList();//fetch all present receptionists
            Receptionist oldCoWorker = Db.Receptionists.SingleOrDefault(g => g.GuestId == GuestId);
            if (allReceptionists == null)
            {
                ViewBag.RightsMessage = "Fetching data did not succed. Please try again.";
                return PartialView("_ConfirmReceptionistRights");
            }
            if (oldCoWorker == null)
            {
                ViewBag.RightsMessage = "The former coworkers receptionist rights is already removed.";
                return PartialView("_ConfirmReceptionistRights");
            }
            for (int i = 0; i < allReceptionists.Count(); i++)
            {
                if (oldCoWorker.GuestId == allReceptionists[i].GuestId)
                {
                    Db.Receptionists.Remove(oldCoWorker);
                }
            }
            if (Db.SaveChanges() != 1)
            {
                ViewBag.RightsMessage = "Saving data did not succed. Please try again.";
                return PartialView("_ConfirmReceptionistRights");
            };
            var user = userManager.FindById(GuestId); //find the former coWorker by Id
            if (user == null)
            {
                ViewBag.RightsMessage = "Did not find the former coworker. Are rights already removed?.";
                return PartialView("_ConfirmReceptionistRights");
            }
            userManager.AddToRole(user.Id, "Guests");//add former coWorker to the role of "Guest"
            userManager.RemoveFromRole(user.Id, "Receptionists");//Remove the role of "Receptionists" from former coworker 
            if (Db.SaveChanges() != 0)
            {
                ViewBag.RightsMessage = "Saving data did not succed. Please try again.";
                return PartialView("_ConfirmReceptionistRights");
            }
            ViewBag.RightsMessage = "The former coworker is no longer receptionist.";
            return PartialView("_ConfirmReceptionistRights");
        }
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult ListReceptionists() //Gather all Guests that is receptionists
        {
            List<Receptionist> currentReceptionists = Db.Receptionists.ToList();
            List<ApplicationUser> receptionistData = new List<ApplicationUser>();
            if (currentReceptionists == null)
            {
                ViewBag.NumberOfReceptionists = "There is none that have receptionist rights or data was not fetched.";
                return PartialView("_ListReceptionist");
            }
            else
            {
                foreach (var receptionist in currentReceptionists)
                {
                    receptionistData.Add(Db.Users.SingleOrDefault(g => g.GuestId == receptionist.GuestId));
                }
                ViewBag.NumberOfReceptionists = "The following " + currentReceptionists.Count() + " have receptionist rights.";
            }
            if (currentReceptionists.Count != receptionistData.Count)
            {
                ViewBag.NumberOfReceptionists = "Matching data was not fetched. Please try again.";
                return PartialView("_ListReceptionist");
            }
            return PartialView("_ListReceptionist", receptionistData);
        }
    }
}