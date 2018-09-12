using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Camping2000.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int ItemId { get; set; }
        public string GuestId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingEndDate { get; set; }
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }
        public decimal BookingPrice { get; set; }
        public bool BookingNeedsElectricity { get; set; }
        [Required]
        public bool GuestHasReserved { get; set; }
        [Required]
        public bool GuestHasCheckedIn { get; set; }
        public bool BookingIsPaid { get; set; }

        /// <summary>
        /// calculates a span of days. Take leapyears into consideration.
        /// Returns a interger value.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int CalculateNumberOfDays(DateTime startDate, DateTime endDate)
        {
            int numberOfDays;
            if (startDate.Year == endDate.Year)
            {
                numberOfDays = endDate.DayOfYear - startDate.DayOfYear;
            }
            else
            {
                if (DateTime.IsLeapYear(startDate.Year))
                {
                    numberOfDays = 366 - startDate.DayOfYear;
                    numberOfDays = numberOfDays + endDate.DayOfYear;
                }
                else
                {
                    numberOfDays = 365 - startDate.DayOfYear;
                    numberOfDays = numberOfDays + endDate.DayOfYear;
                }
            }
            return numberOfDays;
        }
        /// <summary>
        /// Function that collects Camping spots based upon if power is required
        /// </summary>
        /// <param name="BookingNeedsElectricity"></param>
        /// <returns></returns>
        public static List<Camping> FetchCampingSpots(bool BookingNeedsElectricity)
        {
            Camping2000Db Db = new Camping2000Db();
            List<Camping> allSpots = Db.Camping.ToList();
            if (allSpots == null)
            {
                allSpots[0].CampingSpot = "NoData";
                return allSpots;
            }
            List<Camping> ListOfSpots = new List<Camping>();
            foreach (var spot in allSpots)
            {
                if (spot.CampingElectricity == BookingNeedsElectricity)
                {
                    ListOfSpots.Add(spot);
                }
            }
            return ListOfSpots;
        }

        /// <summary>
        /// Function that checks a decimal type if it is null
        /// </summary>
        /// <param name="Price"></param>
        /// <returns>A value that is not null but 0</returns>
        public static decimal checkForNullReferenceException(decimal Price)
        {
            decimal checkedPrice;
            try { checkedPrice = Price; } catch (NullReferenceException) { checkedPrice = 0; }
            return checkedPrice;
        }

        /// <summary>
        /// A function that checks a Int if it is null
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static int checkForNullReferenceException(int Id)
        {
            int checkedId;
            try { checkedId = Id; } catch (NullReferenceException) { checkedId = 0; }
            return checkedId;
        }

        /// <summary>
        /// A function that checks if a valid sting have been fetched
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Returns a valid string. If not valid "Invalidstring" will be returned.</returns>
        public static string checkForGuestNullReferenceException(string Id)
        {
            string checkedId;
            try { checkedId = Id; } catch (NullReferenceException) { checkedId = "Invalidstring"; }
            return checkedId;
        }
    }
}