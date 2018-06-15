using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000
{
    public class Adress
    {
        public int UserId
        {
            get => default(int);
            set
            {
            }
        }
        [MaxLength(100)]
        public string LivingAdressStreet1
        {
            get => default(string);
            set
            {
            }
        }
        [MaxLength(100)]
        public string LivingAdressStreet2
        {
            get => default(string);
            set
            {
            }
        }
        [MaxLength(100)]
        public string LivingAdressStreet3
        {
            get => default(string);
            set
            {
            }
        }
        [Range(0, 99999)]
        public int LivingAdressZipCode
        {
            get => default(int);
            set
            {
            }
        }
        [MaxLength(100)]
        public string LivingAdressCity
        {
            get => default(string);
            set
            {
            }
        }
        [MaxLength(100)]
        public string PostAdressStreet1
        {
            get => default(string);
            set
            {
            }
        }
        [MaxLength(100)]
        public string PostAdressStreet2
        {
            get => default(string);
            set
            {
            }
        }
        [MaxLength(100)]
        public string PostAdressStreet3
        {
            get => default(string);
            set
            {
            }
        }
        [Range(0,99999)]
        public int PostAdressZipCode
        {
            get => default(int);
            set
            {
            }
        }
        [MaxLength(100)]
        public string PostAdressCity
        {
            get => default(string);
            set
            {
            }
        }
    }
}