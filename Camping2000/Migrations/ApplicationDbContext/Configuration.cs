namespace Camping2000.Migrations.ApplicationDbContext
{
    using Camping2000.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Migrations\ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var db = new Camping2000.Models.ApplicationDbContext();
            var store = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(store);

            roleManager.Create(new IdentityRole("Administrators"));
            roleManager.Create(new IdentityRole("Guests"));
            roleManager.Create(new IdentityRole("Receptionists"));
            db.SaveChanges();

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
