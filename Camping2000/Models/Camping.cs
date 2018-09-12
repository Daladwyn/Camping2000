using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Camping2000.Models
{
    public class Camping : ItemToRent
    {
        [Required]
        public string CampingSpot { get; set; }
        [Required]
        public bool CampingElectricity { get; set; }
        [Required]
        public decimal CampingPrice { get; set; }

        /// <summary>
        /// This function compares a list of spots against a list of numbers and 
        /// removes matching items in the list of spots
        /// </summary>
        /// <param name="ListOfSpots"></param>
        /// <param name="notEligibleSpots"></param>
        /// <returns></returns>
        public static List<Camping> RemoveOccupiedSpots(List<Camping> ListOfSpots, List<int> notEligibleSpots)
        {
            for (int i = ListOfSpots.Count - 1; i >= 0; i--)//remove occupied spots from the list of spots
            {
                for (int y = notEligibleSpots.Count - 1; y >= 0; y--)
                {
                    if (ListOfSpots[i].ItemId == notEligibleSpots[y])
                    {
                        ListOfSpots.Remove(ListOfSpots[i]);
                        if (i >= 1)
                        {
                            i--;
                        }
                        else
                        {
                            return ListOfSpots;
                        }
                    }
                }
            }
            return ListOfSpots;
        }
    }
}