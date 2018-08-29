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

        public DbSet<Camping> Campings { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<LinkBooking> LinkBookings { get; set; }
    }
}