using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Models
{
    public partial class Customers
    {
        public Customers()
        {
            Bookings = new HashSet<Bookings>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string CreditCard { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }

        public void PrintInfo()
        {
            var bookingId = new List<int>();
            bookingId = Bookings.Where(x => x.ToDate < DateTime.Now && x.IsCancelled == false).Select(x => x.Id).ToList();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($" ID: {Id}\n Name: {Name}\n Surname: {Surname}\n Age: {Age}\n Credit Card: {CreditCard}\n Email: {Email.ToLower()}\n Valid bookings ID: {string.Join(",",bookingId)}");
            Console.WriteLine("-----------------------------------------");
        }
    }
}
