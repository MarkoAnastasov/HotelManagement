using System;
using System.Collections.Generic;

namespace HotelManagement.Models
{
    public partial class BookingRooms
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int RoomId { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual Rooms Room { get; set; }
    }
}
