using HotelManagement.Common.Exceptions;
using HotelManagement.Models;
using HotelManagement.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelManagement.Services
{
    public class HotelService
    {
        public BookingsRepository BookingsRepository { get; set; }
        public CustomersRepository CustomersRepository { get; set; }
        public RoomsRepository RoomsRepository { get; set; }

        public HotelService()
        {
            BookingsRepository = new BookingsRepository();
            CustomersRepository = new CustomersRepository();
            RoomsRepository = new RoomsRepository();
        }

        public void Book()
        {
            Customers customer = FindCustomerByEmail();
            var bookings = BookingsRepository.GetAllInclude(x => x.BookingRooms);
            var rooms = RoomsRepository.GetAllInclude(x => x.BookingRooms);
            var maxDate = Convert.ToDateTime("01/01/2021 12:00:00 AM");
            Console.WriteLine($"Max booking date: {maxDate}");
            Console.WriteLine("Please enter Check-In date (format MM/dd/yyyy !):");
            var validFrom = Convert.ToDateTime(Console.ReadLine());
            if (validFrom < DateTime.Now || validFrom > maxDate)
            {
                throw new FlowException("Please enter valid date!");
            }
            Console.WriteLine("Please enter Check-Out date (format MM/dd/yyyy !):");
            var validTo = Convert.ToDateTime(Console.ReadLine());
            if (validTo <= validFrom || validTo > maxDate)
            {
                throw new FlowException("Please enter valid date!");
            }
            var newBooking = new Bookings()
            {
                CustomerId = customer.Id,
                NumberOfDays = int.Parse((validTo - validFrom).TotalDays.ToString()),
                FromDate = validFrom,
                ToDate = validTo,
                IsCancelled = false,
                RefCode = GenerateRefCode(),
                BookingRooms = new List<BookingRooms>()
            };
            var activeBookings = bookings.Where(x => x.ToDate > DateTime.Now && x.ToDate > validFrom).ToList();
            var roomsList = new List<int>();
            foreach (var booking in activeBookings)
            {
                if (booking.IsCancelled != true)
                {
                    roomsList.AddRange(booking.BookingRooms.Select(x => x.RoomId).ToList());
                }
            }
            Console.WriteLine(string.Join(",",roomsList));
            bool anotherRoom = true;
            while (anotherRoom)
            {
                PrintAvailRooms(roomsList, rooms);
                Rooms room = FindRoom();
                var bookedRoom = new BookingRooms()
                {
                    RoomId = room.Id,
                    BookingId = newBooking.Id
                };
                bool available = CheckRoomAvail(roomsList, room);
                if (available == false)
                {
                    throw new FlowException("Room is not available!");
                }
                var checkRoom = newBooking.BookingRooms.FirstOrDefault(x => x.RoomId == room.Id);
                if(checkRoom != null)
                {
                    throw new FlowException("You already have this room!");
                }
                Console.WriteLine($"This room has {room.MaxPersons} max persons allowed! You may need to book another room if needed!");
                Console.WriteLine("Please enter number of persons:");
                var checkPersons = int.TryParse(Console.ReadLine(), out int roomPersons);
                CheckInput(checkPersons);
                CheckAboveZero(roomPersons);
                if (roomPersons > room.MaxPersons)
                {
                    throw new FlowException("Please check room details!");
                }
                newBooking.BookingRooms.Add(bookedRoom);
                Console.WriteLine("Do you want to add another room to your booking? Enter y if yes,any other key if no!");
                var userChoice = Console.ReadLine();
                anotherRoom = userChoice.ToLower() == "y";
            }
            Console.WriteLine($"You have booked successfully! Your Reference Code is: {newBooking.RefCode}");
            BookingsRepository.Add(newBooking);
            BookingsRepository.SaveEntities();
        }

        private static bool CheckRoomAvail(List<int> roomsList, Rooms room)
        {
            var available = true;
            foreach (var hotelRoom in roomsList)
            {
                if (hotelRoom == room.Id)
                {
                    available = false;
                    break;
                }
            }
            return available;
        }

        private static void PrintAvailRooms(List<int> roomsList,List<Rooms> rooms)
        {
            foreach (var room in rooms)
            {
                if (roomsList.Contains(room.Id))
                {
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine($"Room with Id: {room.Id} is not available for this duration!");
                    Console.WriteLine("-----------------------------------------");
                }
                else
                {
                    room.PrintInfo();
                }
            }
        }

        private static string GenerateRefCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var referenceCode = new string(stringChars);
            return referenceCode;
        }

        public void RegisterCustomer()
        {
            var customers = CustomersRepository.GetAll();
            string customerName, customerSurname, customerCard;
            int customerAge;
            CustomerInputModel(out customerName, out customerSurname, out customerAge, out customerCard);
            Console.WriteLine("Please enter your E-mail:");
            var customerMail = Console.ReadLine().ToUpper();
            var customer = customers.FirstOrDefault(x => x.Email == customerMail);
            if (customer != null)
            {
                throw new FlowException("Customer already exists!");
            }
            var newCustomer = new Customers()
            {
                Name = customerName,
                Surname = customerSurname,
                Age = customerAge,
                CreditCard = customerCard,
                Email = customerMail
            };
            Console.WriteLine("You have registered successfully! Don't forget to make your first booking :)");
            CustomersRepository.Add(newCustomer);
            CustomersRepository.SaveEntities();
        }

        private void CustomerInputModel(out string customerName, out string customerSurname, out int customerAge, out string customerCard)
        {
            Console.WriteLine("Please enter your Name:");
            customerName = Console.ReadLine();
            Console.WriteLine("Please enter your Surname:");
            customerSurname = Console.ReadLine();
            Console.WriteLine("Please enter your Age:");
            var checkAge = int.TryParse(Console.ReadLine(), out customerAge);
            CheckInput(checkAge);
            if (customerAge <= 0 || customerAge < 18)
            {
                throw new FlowException("Please enter age above 18!");
            }
            Console.WriteLine("Please enter your credit card number:");
            customerCard = Console.ReadLine();
        }

        public void EditProfile()
        {
            Customers customer = FindCustomerByEmail();
            string customerName, customerSurname, customerCard;
            int customerAge;
            CustomerInputModel(out customerName, out customerSurname, out customerAge, out customerCard);
            customer.Name = customerName;
            customer.Surname = customerSurname;
            customer.Age = customerAge;
            customer.CreditCard = customerCard;
            CustomersRepository.Update(customer);
            CustomersRepository.SaveEntities();
            Console.WriteLine("Profile updated successfully!");
        }

        private Customers FindCustomerByEmail()
        {
            Console.WriteLine("Please enter your e-mail:");
            var userEmail = Console.ReadLine();
            var customers = CustomersRepository.GetAll();
            var customer = customers.FirstOrDefault(x => x.Email == userEmail);
            if (customer == null)
            {
                throw new FlowException("Customer not found!");
            }
            return customer;
        }

        public void CancelBooking()
        {
            Customers customer = FindCustomerByEmail();
            Console.WriteLine("Please enter booking reference code:");
            var refCode = Console.ReadLine();
            var bookings = BookingsRepository.GetAll();
            var booking = bookings.FirstOrDefault(x => x.RefCode == refCode && x.FromDate > DateTime.Now && x.CustomerId == customer.Id);
            if(booking == null)
            {
                throw new FlowException("Booking not found!");
            }
            Console.WriteLine($"Confirm you want to cancel booking {booking.RefCode}? Enter y if yes!");
            var confirm = Console.ReadLine();
            if (confirm.ToLower() == "y")
            {
                booking.IsCancelled = true;
            }
            BookingsRepository.Update(booking);
            BookingsRepository.SaveEntities();
            Console.WriteLine("Booking is cancelled!Please check your credit card after 10 minutes!");
        }

        public void AddRoom()
        {
            int roomPremises, floorNumber, maxPersons;
            decimal priceDaily;
            string hasWifi, NonSmoking;
            RoomInputModel(out roomPremises, out floorNumber, out priceDaily, out hasWifi, out NonSmoking, out maxPersons);
            var newRoom = new Rooms()
            {
                RoomType = roomPremises,
                FloorNumber = floorNumber,
                IsBooked = false,
                MaxPersons = maxPersons,
                PricePerDay = priceDaily
            };
            RoomProperties(hasWifi, NonSmoking, newRoom);
            RoomsRepository.Add(newRoom);
            RoomsRepository.SaveEntities();
            Console.WriteLine("Room successfully added!");
        }

        private void RoomInputModel(out int roomPremises, out int floorNumber, out decimal priceDaily, out string hasWifi, out string NonSmoking, out int maxPersons)
        {
            Console.WriteLine("Please enter number of premises:");
            var checkPremises = int.TryParse(Console.ReadLine(), out roomPremises);
            CheckInput(checkPremises);
            if (roomPremises <= 0 || roomPremises > 7)
            {
                throw new FlowException("Number of premises is invalid!");
            }
            Console.WriteLine("Please enter floor number:");
            var checkFloor = int.TryParse(Console.ReadLine(), out floorNumber);
            CheckInput(checkFloor);
            if (floorNumber <= 0 || floorNumber > 5)
            {
                throw new FlowException("Floor number is invalid!");
            }
            Console.WriteLine("Please enter price per day (EUR):");
            var checkPrice = decimal.TryParse(Console.ReadLine(), out priceDaily);
            CheckInput(checkPrice);
            CheckAboveZero(priceDaily);
            Console.WriteLine("Enter y if this room has free WiFi:");
            hasWifi = Console.ReadLine().ToLower();
            Console.WriteLine("Enter y if this room is non smoking:");
            NonSmoking = Console.ReadLine().ToLower();
            Console.WriteLine("Please enter maximum persons allowed:");
            var checkPersons = int.TryParse(Console.ReadLine(), out maxPersons);
            CheckInput(checkPersons);
            CheckAboveZero(maxPersons);
        }

        public void EditRoom()
        {
            Rooms room = FindRoom();
            int roomPremises, floorNumber, maxPersons;
            decimal priceDaily;
            string hasWifi, NonSmoking;
            RoomInputModel(out roomPremises, out floorNumber, out priceDaily, out hasWifi, out NonSmoking, out maxPersons);
            RoomProperties(hasWifi, NonSmoking, room);
            room.PricePerDay = priceDaily;
            room.MaxPersons = maxPersons;
            room.FloorNumber = floorNumber;
            room.RoomType = roomPremises;
            RoomsRepository.Update(room);
            RoomsRepository.SaveEntities();
            Console.WriteLine("Room successfully edited!");
        }

        public void GetValidBookings()
        {
            var bookings = BookingsRepository.GetAll();
            var validBookings = bookings.Where(x => x.ToDate < DateTime.Now && x.IsCancelled == false).ToList();
            validBookings.ForEach(x => x.PrintInfo());
        }

        public void GetRooms()
        {
            var rooms = RoomsRepository.GetAll();
            rooms.ForEach(x => x.PrintInfo());
        }

        private Rooms FindRoom()
        {
            Console.WriteLine("Please enter room ID: ");
            var checkRoomId = int.TryParse(Console.ReadLine(), out int roomId);
            CheckInput(checkRoomId);
            var room = RoomsRepository.GetById(roomId);
            if (room == null)
            {
                throw new FlowException("Room not found!");
            }
            return room;
        }

        public void GetCustomers()
        {
            var customers = CustomersRepository.GetAll();
            customers.ForEach(x => x.PrintInfo());
        }

        public void GetBookingById()
        {
            Console.WriteLine("Please enter booking ID:");
            var checkBookingId = int.TryParse(Console.ReadLine(), out int bookingId);
            CheckInput(checkBookingId);
            var booking = BookingsRepository.GetById(bookingId);
            if (booking == null)
            {
                throw new FlowException("Booking not found!");
            }
            booking.PrintInfo();
        }

        public void GetRoomById()
        {
            Rooms room = FindRoom();
            room.PrintInfo();
        }

        public void GetCustomerById()
        {
            Console.WriteLine("Please enter customer ID:");
            var checkCustomerId = int.TryParse(Console.ReadLine(), out int customerId);
            CheckInput(checkCustomerId);
            var customer = CustomersRepository.GetById(customerId);
            if (customer == null)
            {
                throw new FlowException("Customer not found!");
            }
            customer.PrintInfo();
        }

        public void CheckIfAdmin()
        {
            Console.WriteLine("Please enter your password:");
            var password = Console.ReadLine();
            if(password != "admin123")
            {
                throw new FlowException("Password is incorrect!");
            }
        }

        private static void RoomProperties(string hasWifi, string NonSmoking, Rooms newRoom)
        {
            if (hasWifi == "y")
            {
                newRoom.HasFreeWifi = true;
            }
            else
            {
                newRoom.HasFreeWifi = false;
            }
            if (NonSmoking == "y")
            {
                newRoom.NonSmoking = true;
            }
            else
            {
                newRoom.NonSmoking = false;
            }
        }

        private void CheckInput(bool input)
        {
            if(input == false)
            {
                throw new FlowException("Input is invalid! Please enter a number!");
            }
        }

        private static void CheckAboveZero(decimal input)
        {
            if (input <= 0)
            {
                throw new FlowException("Please enter number above 0!");
            }
        }
    }
}
