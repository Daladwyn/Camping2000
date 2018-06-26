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

        }
        public DbSet<Camping> Camping { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Adress> Adresses { get; set; }
    }
}