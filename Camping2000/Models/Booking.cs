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
        [Required]
        public int ItemId
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public int GuestId
        {
            get => default(int);
            set
            {
            }
        }
        [Key]
        public int BookingId
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public DateTime BookingStartDate
        {
            get => default(DateTime);
            set
            {
            }
        }
        [Required]
        public DateTime BookingEndDate
        {
            get => default(DateTime);
            set
            {
            }
        }
        public int NumberOfGuests { get; set; }
        public decimal BookingPrice { get; set; }
    }
}