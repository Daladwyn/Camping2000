using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class BookingGuestViewModel
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string GuestId { get; set; }
        [Required]
        public string GuestFirstName { get; set; }
        [Required]
        public string GuestLastName { get; set; }
        [Required]
        public decimal BookingPrice { get; set; }
        public int NumberOfGuests { get; set; }
        [Required]
        [MaxLength(40)]
        public string ItemName { get; set; }
    }
}