namespace Camping2000.Migrations
{
    using Camping2000.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Camping2000.Models.Camping2000Db>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Camping2000.Models.Camping2000Db context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Camping.AddOrUpdate(i => i.ItemName,
                  new Camping { ItemName = "Camping Spot1", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "1", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot2", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "2", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot3", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "3", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot4", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "4", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot5", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "5", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot6", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "6", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot7", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "7", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot8", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "8", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot9", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "9", CampingPrice = 50, CampingElectricity = false },
                  new Camping { ItemName = "Camping Spot10", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "10", CampingPrice = 50, CampingElectricity = false });
            context.SaveChanges();

            context.Camping.AddOrUpdate(i => i.ItemName,
                 new Camping { ItemName = "Trailer Spot1", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "11", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot2", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "12", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot3", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "13", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot4", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "14", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot5", ItemIsOccupied = false, ItemNumberOfPersons = 10, CampingSpot = "15", CampingPrice = 100, CampingElectricity = true });
            context.SaveChanges();

            //var db = new ();
            var store = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(store);

            roleManager.Create(new IdentityRole("Administrators"));
            roleManager.Create(new IdentityRole("Guests"));
            roleManager.Create(new IdentityRole("Receptionists"));
            context.SaveChanges();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.Create(user: new ApplicationUser()
            {
                UserName = "Admin@camping.com",//Change emailadress before putting this bookingsystem into production
                Email = "admin@camping.com",

            }, password: "Test!0");//Change password before putting this bookingsystem into production
            context.SaveChanges();
            var user = userManager.FindByName("Admin@camping.com");
            userManager.AddToRole(user.Id, "Administrators");
            context.SaveChanges();
        }
    }
}
