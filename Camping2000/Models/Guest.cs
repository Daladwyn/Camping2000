using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camping2000.Models
{
    public class Guest
    {
        [Key]
        public string GuestId { get; set; } //needs to be string to match identitys format for userId
        [Required]
        [MaxLength(80)]
        public string GuestFirstName { get; set; }
        [Required]
        [MaxLength(80)]
        public string GuestLastName { get; set; }
        [Required]
        [MaxLength(40)]
        public string GuestNationality { get; set; }
        [Required]
        public bool GuestHasReserved { get; set; }
        [Required]
        public bool GuestHasCheckedIn { get; set; }
        public decimal GuestHasToPay { get; set; } //this property is used for gathering the amount the guest have to pay.
        public decimal GuestHasPaid { get; set; }// This property is used as a checker if the guest have paid.
        [MaxLength(20)]
        public string GuestPhoneNumber { get; set; }
        [MaxLength(20)]
        public string GuestMobileNumber { get; set; }// public int UserTelephoneNr { get; set; } Is located in ManageViewModel as PhoneNumber
    }
}