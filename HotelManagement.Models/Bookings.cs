using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Models
{
    public partial class Bookings
    {
        public Bookings()
        {
            BookingRooms = new HashSet<BookingRooms>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsCancelled { get; set; }
        public string RefCode { get; set; }
        public virtual Customers Customer { get; set; }
        public virtual ICollection<BookingRooms> BookingRooms { get; set; } 

        public void PrintInfo()
        {
            var totalRoomsPrice = 0.0m;
            var currentRooms = BookingRooms.Select(x => x.Room).ToList();
            foreach (var room in currentRooms)
            {
                Console.WriteLine(room.Id);
            }
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($" ID: {Id}\n Customer ID: {CustomerId}\n From: {FromDate}\n To: {ToDate}\n Reference code: {RefCode}\n Total price: {NumberOfDays*totalRoomsPrice} EUR");
            Console.WriteLine("-----------------------------------------");
        }
    }
}
