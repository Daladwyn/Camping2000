using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int ItemId { get; set; }
        public string GuestId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingEndDate { get; set; }
        [Range(1,10)]
        public int NumberOfGuests { get; set; }
        public decimal BookingPrice { get; set; }
        public bool BookingNeedsElectricity { get; set; }
        [Required]
        public bool GuestHasReserved { get; set; }
        [Required]
        public bool GuestHasCheckedIn { get; set; }
    }
}