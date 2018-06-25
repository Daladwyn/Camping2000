using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Camping2000.Models
{
    public class SmallCabin : ItemToRent
    {
        public int NumberOfBeds
        {
            get => default(int);
            set
            {
            }
        }

        public decimal PriceFor2
        {
            get => default(decimal);
            set
            {
            }
        }

        public decimal PriceFor4
        {
            get => default(decimal);
            set
            {
            }
        }
    }
}