namespace Camping2000.Migrations.ApplicationDbContext
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Camping2000.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Migrations\ApplicationDbContext";
        }

        protected override void Seed(Camping2000.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Roles.AddOrUpdate(r => r.Name,
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("User"),
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin"));
            context.SaveChanges();

            //context.Users.AddOrUpdate(u => u.UserName,
            //    new Microsoft.AspNet.Identity.EntityFramework.IdentityUser() { UserName="admin", C ="Admin",  "admin@camp.com" });
        }
    }
}
