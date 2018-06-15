using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Camping2000
{
    public class Camping : ItemToRent
    {
        [Required]
        public string CampingSpot
        {
            get => default(string);
            set
            {
            }
        }
        [Required]
        public bool CampingElectricity
        {
            get => default(bool);
            set
            {
            }
        }
        [Required]
        public int CampingPrice
        {
            get => default(int);
            set
            {
            }
        }
    }
}