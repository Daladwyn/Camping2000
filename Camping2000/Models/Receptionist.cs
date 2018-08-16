using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Camping2000.Models
{
    public class Receptionist

    {
        [Key]
        public int ReceptionistId { get; set; }
        public string GuestId { get; set; }
    }
}