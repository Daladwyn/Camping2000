namespace Camping2000.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class camping2000 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adresses",
                c => new
                    {
                        AdressId = c.Int(nullable: false, identity: true),
                        GuestId = c.String(nullable: false),
                        LivingAdressStreet1 = c.String(maxLength: 100),
                        LivingAdressStreet2 = c.String(maxLength: 100),
                        LivingAdressStreet3 = c.String(maxLength: 100),
                        LivingAdressZipCode = c.Int(nullable: false),
                        LivingAdressCity = c.String(maxLength: 100),
                        PostAdressStreet1 = c.String(maxLength: 100),
                        PostAdressStreet2 = c.String(maxLength: 100),
                        PostAdressStreet3 = c.String(maxLength: 100),
                        PostAdressZipCode = c.Int(nullable: false),
                        PostAdressCity = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AdressId);
            
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        GuestId = c.String(),
                        BookingStartDate = c.DateTime(nullable: false),
                        BookingEndDate = c.DateTime(nullable: false),
                        NumberOfGuests = c.Int(nullable: false),
                        BookingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookingNeedsElectricity = c.Boolean(nullable: false),
                        GuestHasReserved = c.Boolean(nullable: false),
                        GuestHasCheckedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
            CreateTable(
                "dbo.Campings",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        CampingSpot = c.String(nullable: false),
                        CampingElectricity = c.Boolean(nullable: false),
                        CampingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemName = c.String(nullable: false, maxLength: 40),
                        ItemNumberOfPersons = c.Int(nullable: false),
                        ItemIsOccupied = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId);
            
           
            
            CreateTable(
                "dbo.LinkBookings",
                c => new
                    {
                        LinkBookingId = c.Int(nullable: false, identity: true),
                        PreBooking = c.Int(nullable: false),
                        PostBooking = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LinkBookingId);
            
            CreateTable(
                "dbo.Receptionists",
                c => new
                    {
                        ReceptionistId = c.Int(nullable: false, identity: true),
                        GuestId = c.String(),
                    })
                .PrimaryKey(t => t.ReceptionistId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Receptionists");
            DropTable("dbo.LinkBookings");
            DropTable("dbo.Guests");
            DropTable("dbo.Campings");
            DropTable("dbo.Bookings");
            DropTable("dbo.Adresses");
        }
    }
}
