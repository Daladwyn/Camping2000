using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000
{
    public class User
    {
        [Key]
        public int UserId
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public string UserFirstName
        {
            get => default(string);
            set
            {
            }
        }
        [Required]
        public string UserNationality
        {
            get => default(string);
            set
            {
            }
        }
        [Required]
        public string UserMailAdress
        {
            get => default(string);
            set
            {
            }
        }
        [Required]
        public bool UserHasReserved
        {
            get => default(bool);
            set
            {
            }
        }
        [Required]
        public bool UserHasCheckedIn
        {
            get => default(bool);
            set
            {
            }
        }
       
        public int UserHasToPay
        {
            get => default(int);
            set
            {
            }
        }

        public int UserHasPaid
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public string UserLastName
        {
            get => default(string);
            set
            {
            }
        }
        public int UserTelephoneNr { get; set; }
        public int UserMobileNr { get; set; }
    }
}