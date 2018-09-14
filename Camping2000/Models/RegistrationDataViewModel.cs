using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class RegistrationDataViewModel
    {
        [Required]
        public string GuestId { get; set; } //needs to be string to match identitys format for userId

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "Your Firstname *")]
        [Required(ErrorMessage = "Your firstname is required.")]
        [MaxLength(80)]
        public string GuestFirstName { get; set; }

        [Display(Name ="Your Country *")]
        [Required(ErrorMessage = "Your country is required.")]
        [MaxLength(40)]
        public string GuestNationality { get; set; }

        [Display(Name = "Your Lastname *")]
        [Required(ErrorMessage = "Your lastname is required.")]
        [MaxLength(80)]
        public string GuestLastName { get; set; }

        [Phone]
        [Display(Name = "Your Phonenumber")]
        public string GuestPhoneNumber { get; set; }

        [Phone]
        [Display(Name = "Your Mobilenumber *")]
        [Required(ErrorMessage = "Your mobilenumber is required.")]
        public string GuestMobileNumber { get; set; }

        public int AdressId { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Homeadress")]
        public string LivingAdressStreet1 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Homeadress")]
        public string LivingAdressStreet2 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Homeadress")]
        public string LivingAdressStreet3 { get; set; }

        [Range(0, 99999, ErrorMessage = "Only exactly 5 digits is allowed in this field.")]
        //[RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Specified livingadress zipcode is not valid. Try again.")]
        [Display(Name = "Your ZipCode")]
        public int LivingAdressZipCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Hometown")]
        public string LivingAdressCity { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Postadress")]
        public string PostAdressStreet1 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Postadress")]
        public string PostAdressStreet2 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Postadress")]
        public string PostAdressStreet3 { get; set; }

        [Range(0, 99999,ErrorMessage ="Only exactly 5 digits is allowed in this field.")]
        //[RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Specified postadress zipcode is not valid. Try again.")]
        [Display(Name = "Your Postzipcode")]
        public int PostAdressZipCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "Your Posttown")]
        public string PostAdressCity { get; set; }
    }
}