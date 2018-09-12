using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Camping2000.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        public string GuestId { get; set; }
        //[Required]
        [MaxLength(80)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "First name is invalid.")]
        public string GuestFirstName { get; set; }
       // [Required]
        [MaxLength(80)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Last name is invalid.")]
        public string GuestLastName { get; set; }
      //  [Required]
        [MaxLength(40)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Nationality is invalid.")]
        public string GuestNationality { get; set; }
        //[Required]
        public bool GuestHasReserved { get; set; }
       // [Required]
        public bool GuestHasCheckedIn { get; set; }
        public decimal GuestHasToPay { get; set; } //this property is used for gathering the amount the guest have to pay.
        public decimal GuestHasPaid { get; set; }// This property is used as a checker if the guest have paid.
        [MaxLength(20)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Phone number is invalid.")]
        public string GuestPhoneNumber { get; set; }
        [MaxLength(20)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Mobile number is invalid.")]
        public string GuestMobileNumber { get; set; }// public int UserTelephoneNr { get; set; } Is located in ManageViewModel as PhoneNumber

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class Camping2000Db : IdentityDbContext<ApplicationUser>
    {
        public Camping2000Db() : base("Camping2000Db", throwIfV1Schema: false)
        {
        }

        public static Camping2000Db Create()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Camping2000Db, Migrations.Configuration>());
            return new Camping2000Db();
        }

        public DbSet<Camping> Camping { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<LinkBooking> LinkBookings { get; set; }

        // /// <summary>
        // /// Function that accepts one or two strings and return a list of guests 
        // /// </summary>
        // /// <param name="firstName"></param>
        // /// <param name="lastName"></param>
        // /// <returns></returns>
        public static List<ApplicationUser> SearchForPeople(string firstName, string lastName)
        {
            Camping2000Db Db = new Camping2000Db();
            List<ApplicationUser> foundGuests = new List<ApplicationUser>();
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            if ((firstName != "") && (lastName == ""))
            {
                foreach (var guest in Db.Users)
                {
                    if (guest.GuestFirstName.ToLower() == firstName)
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            else if ((firstName != "") && (lastName != ""))
            {
                foreach (var guest in Db.Users)
                {
                    if ((guest.GuestFirstName.ToLower() == firstName) && (guest.GuestLastName.ToLower() == lastName))
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            else if ((firstName == "") && (lastName != ""))
            {
                foreach (var guest in Db.Users)
                {
                    if (guest.GuestLastName.ToLower() == lastName)
                    {
                        foundGuests.Add(guest);
                    }
                }
            }
            return foundGuests;
        }
    }
}