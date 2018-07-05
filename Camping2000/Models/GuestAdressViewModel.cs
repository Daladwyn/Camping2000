using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class GuestAdressViewModel
    {
        [Required]
        public string GuestId { get; set; } //needs to be string to match identitys format for userId
        [MaxLength(80)]
        public string GuestFirstName { get; set; }
        [MaxLength(80)]
        public string GuestLastName { get; set; }
        [MaxLength(40)]
        public string GuestNationality { get; set; }
        [MaxLength(20)]
        public string GuestPhoneNumber { get; set; }
        [MaxLength(20)]
        public string GuestMobileNumber { get; set; }// public int UserTelephoneNr { get; set; } Is located in ManageViewModel as PhoneNumber
        public int AdressId { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet1 { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet2 { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet3 { get; set; }
        [Range(0, 99999)]
        public int LivingAdressZipCode { get; set; }
        [MaxLength(100)]
        public string LivingAdressCity { get; set; }
        [MaxLength(100)]
        public string PostAdressStreet1 { get; set; }
        [MaxLength(100)]
        public string PostAdressStreet2 { get; set; }
        [MaxLength(100)]
        public string PostAdressStreet3 { get; set; }
        [Range(0, 99999)]
        public int PostAdressZipCode { get; set; }
        [MaxLength(100)]
        public string PostAdressCity { get; set; }
    }
}