using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class GuestDataViewModel
    {
        [Required]
        public string GuestId { get; set; } //needs to be string to match identitys format for userId
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(80)]
        [Display(Name ="Guest Firstname")]
        public string GuestFirstName { get; set; }
        [Required]
        [MaxLength(40)]
        public string GuestNationality { get; set; }
        [MaxLength(80)]
        [Display(Name = "Guest Lastname")]
        public string GuestLastName { get; set; }
        [Phone]
        public string GuestPhoneNumber { get; set; }
        [Phone]
        public string GuestMobileNumber { get; set; }
        public int AdressId { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet1 { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet2 { get; set; }
        [MaxLength(100)]
        public string LivingAdressStreet3 { get; set; }
        [Range(0, 99999)]
       // [RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Specified livingadress zipcode is not valid. Try again.")]
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
        //[RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Specified postadress zipcode is not valid. Try again.")]
        public int PostAdressZipCode { get; set; }
        [MaxLength(100)]
        public string PostAdressCity { get; set; }
    }
}