using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000.Models
{
    public class Adress
    {
        [Key]
        public int AdressId { get; set; }
        [Required]
        public string GuestId { get; set; }
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