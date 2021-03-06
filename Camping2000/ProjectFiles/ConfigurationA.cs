namespace Camping2000.Migrations.ApplicationDbContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Camping2000.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ApplicationDbContext";
        }

        protected override void Seed(Camping2000.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var db = new ApplicationDbContext();
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
