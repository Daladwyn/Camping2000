using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class Camping2000Db : DbContext
    {
        public Camping2000Db(): base("Camping2000Db")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Camping2000Db, Migrations.Camping2000Db.Configuration>());
        }
        public DbSet<Camping> Camping { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<LinkBooking> LinkBookings { get; set; }
    }
}