using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000
{
    public class ItemToRent
    {
        [Key]
        public int ItemId
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        [MaxLength(40)]
        public string ItemName
        { get; set; }


        [Range(1, 10)]
        public int ItemNumberOfPersons
        {
            get => default(int);
            set
            {
            }
        }
        [Required]
        public bool ItemIsBooked { get; set; }
    }
}