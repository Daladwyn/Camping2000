using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class ModifyBookingViewModel
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        public string GuestId { get; set; }
                public string GuestFirstName { get; set; }
                public string GuestLastName { get; set; }
        [Required]
        public int ItemId { get; set; }
                public string ItemName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingEndDate { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        public decimal BookingPrice { get; set; }
        [Required]
        public bool BookingNeedsElectricity { get; set; }
        public List<Camping> VacantSpots = new List<Camping>();
        public bool GuestHasCheckedIn { get; set; }
    }

}