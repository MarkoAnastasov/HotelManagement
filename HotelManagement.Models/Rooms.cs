using System;
using System.Collections.Generic;

namespace HotelManagement.Models
{
    public partial class Rooms
    {
        public Rooms()
        {
            BookingRooms = new HashSet<BookingRooms>();
        }

        public int Id { get; set; }
        public int RoomType { get; set; }
        public int FloorNumber { get; set; }
        public bool IsBooked { get; set; }
        public bool HasFreeWifi { get; set; }
        public bool NonSmoking { get; set; }
        public int MaxPersons { get; set; }
        public decimal PricePerDay { get; set; }

        public virtual ICollection<BookingRooms> BookingRooms { get; set; }

        public void PrintInfo()
        {
            var wifiInfo = "";
            var smokingInfo = "";
            var bookedInfo = "";
            wifiInfo = CheckProp(HasFreeWifi,wifiInfo);
            smokingInfo = CheckProp(NonSmoking, smokingInfo);
            bookedInfo = CheckProp(IsBooked, bookedInfo);
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($" ID: {Id}\n Number of premises: {RoomType}\n Max persons: {MaxPersons}\n Floor: {FloorNumber}\n Has WiFi free: {wifiInfo}\n Is non smoking: {smokingInfo}\n Price per day: {String.Format("{0:n}", PricePerDay)} EUR");
            Console.WriteLine("-----------------------------------------");
        }

        private string CheckProp(bool input,string result)
        {
            if (input == true)
            {
                result = "Yes";
            }
            else
            {
                result = "No";
            }

            return result;
        }
    }
}
