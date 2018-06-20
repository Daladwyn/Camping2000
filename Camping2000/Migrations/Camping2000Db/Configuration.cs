namespace Camping2000.Migrations.Camping2000Db
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Camping2000.Models.Camping2000Db>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Migrations\Camping2000Db";
        }

        protected override void Seed(Camping2000.Models.Camping2000Db context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Camping.AddOrUpdate(i => i.ItemId,
                new Camping { ItemName = "Camping Spot1", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "1", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot2", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "2", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot3", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "3", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot4", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "4", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot5", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "5", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot6", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "6", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot7", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "7", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot8", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "8", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot9", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "9", CampingPrice = 50, CampingElectricity = false },
                new Camping { ItemName = "Camping Spot10", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "10", CampingPrice = 50, CampingElectricity = false });
            context.SaveChanges();

            context.Camping.AddOrUpdate(i => i.ItemId,
                 new Camping { ItemName = "Trailer Spot1", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "11", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot2", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "12", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot3", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "13", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot4", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "14", CampingPrice = 100, CampingElectricity = true },
                 new Camping { ItemName = "Trailer Spot5", ItemIsBooked = false, ItemNumberOfPersons = 10, CampingSpot = "15", CampingPrice = 100, CampingElectricity = true });
            context.SaveChanges();


        }
    }
}
