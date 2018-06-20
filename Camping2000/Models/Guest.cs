using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000
{
    public class Guest
    {
        [Key]
        public int GuestId
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public string GuestFirstName
        {
            get => default(string);
            set
            {
            }
        }
        [Required]
        public string GuestNationality
        {
            get => default(string);
            set
            {
            }
        }
        //[Required] Is located in AccountViewModels as Email
        //public string GuestMailAdress
        //{
        //    get => default(string);
        //    set
        //    {
        //    }
        //}
        [Required]
        public bool GuestHasReserved
        {
            get => default(bool);
            set
            {
            }
        }
        [Required]
        public bool GuestHasCheckedIn
        {
            get => default(bool);
            set
            {
            }
        }
       
        public decimal GuestHasToPay //this property is used for gathering the amount the guest have to pay.
        {
            get => default(int);
            set
            {
            }
        }

        public decimal GuestHasPaid // This property is used as a checker if the guest have paid.
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public string GuestLastName
        {
            get => default(string);
            set
            {
            }
        }
        // public int UserTelephoneNr { get; set; } Is located in ManageViewModel as PhoneNumber
        public int GuestMobileNr { get; set; }
    }
}