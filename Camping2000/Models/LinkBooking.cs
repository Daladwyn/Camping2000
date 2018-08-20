using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class LinkBooking
    {
        [Key]
        public int LinkBookingId { get; set; }
        public int PreBooking { get; set; }
        public int PostBooking { get; set; }
    }
}