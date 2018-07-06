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
        public int ItemId { get; set; }
        public string GuestId { get; set; }
        [MaxLength(80)]
        public string GuestFirstName { get; set; }
        [MaxLength(80)]
        public string GuestLastName { get; set; }
        public decimal BookingPrice { get; set; }
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }
        [MaxLength(40)]
        public string ItemName { get; set; }
    }
}