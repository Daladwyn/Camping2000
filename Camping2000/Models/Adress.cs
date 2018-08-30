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
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Living address is invalid.")]
        public string LivingAdressStreet1 { get; set; }
        [MaxLength(100)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Living address is invalid.")]
        public string LivingAdressStreet2 { get; set; }
        [MaxLength(100)]
       // [RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Living address is invalid.")]
        public string LivingAdressStreet3 { get; set; }
        [Range(0, 99999)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Living address zipcode is invalid.")]
        public int LivingAdressZipCode { get; set; }
        [MaxLength(100)]
       // [RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Living address city is invalid.")]
        public string LivingAdressCity { get; set; }
        [MaxLength(100)]
       //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Post address is invalid.")]
        public string PostAdressStreet1 { get; set; }
        [MaxLength(100)]
        //[RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Post address is invalid.")]
        public string PostAdressStreet2 { get; set; }
        [MaxLength(100)]
       // [RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Post address is invalid.")]
        public string PostAdressStreet3 { get; set; }
        [Range(0, 99999)]
       // [RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Post adress zipcode is invalid.")]
        public int PostAdressZipCode { get; set; }
        [MaxLength(100)]
       // [RegularExpression("^[<>.!@#%/]+$", ErrorMessage = "Post address city is invalid.")]
        public string PostAdressCity { get; set; }
    }
}