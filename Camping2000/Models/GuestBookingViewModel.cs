using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class GuestBookingViewModel
    {
        [Required]
        public string GuestId { get; set; } //needs to be string to match identitys format for userId
        [Required]
        [MaxLength(80)]
        public string GuestFirstName { get; set; }
        [Required]
        [MaxLength(80)]
        public string GuestLastName { get; set; }
        [Phone]
        public string GuestPhoneNumber { get; set; }
        [Phone]
        public string GuestMobileNumber { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingEndDate { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public decimal BookingPrice { get; set; }
        [Required]
        public bool BookingNeedsElectricity { get; set; }
    }
}