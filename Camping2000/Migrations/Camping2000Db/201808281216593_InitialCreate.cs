namespace Camping2000.Migrations.Camping2000Db
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                "dbo.Guests",
                c => new
                    {
                        GuestId = c.String(nullable: false, maxLength: 128),
                        GuestFirstName = c.String(nullable: false, maxLength: 80),
                        GuestLastName = c.String(nullable: false, maxLength: 80),
                        GuestNationality = c.String(nullable: false, maxLength: 40),
                        GuestHasReserved = c.Boolean(nullable: false),
                        GuestHasCheckedIn = c.Boolean(nullable: false),
                        GuestHasToPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GuestHasPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GuestPhoneNumber = c.String(maxLength: 20),
                        GuestMobileNumber = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.GuestId);
            
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Receptionists");
            DropTable("dbo.LinkBookings");
            DropTable("dbo.Guests");
            DropTable("dbo.Campings");
            DropTable("dbo.Bookings");
            DropTable("dbo.Adresses");
        }
    }
}
