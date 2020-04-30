using HotelManagement.Common.Exceptions;
using HotelManagement.Services;
using System;

namespace HotelManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            var hotelService = new HotelService();
            var continueProgram = true;
            while (continueProgram)
            {
                try
                {
                    Console.WriteLine("Please choose the following options:");
                    Console.WriteLine(" 1.Book rooms\n 2.Register\n 3.Edit profile\n 4.Cancel booking\n 5.Administrator only");
                    int.TryParse(Console.ReadLine(), out int userChoice);
                    switch (userChoice)
                    {
                        case 1:
                            hotelService.Book();
                            break;
                        case 2:
                            hotelService.RegisterCustomer();
                            break;
                        case 3:
                            hotelService.EditProfile();
                            break;
                        case 4:
                            hotelService.CancelBooking();
                            break;
                        case 5:
                            hotelService.CheckIfAdmin();
                            ShowAdminMenu(hotelService);
                            break;
                        default:
                            Console.WriteLine("Option not found!");
                            break;
                    }
                }
                catch (FlowException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("An error has occured! Please try again later!");
                }
                Console.WriteLine("Do you want to continue the program?Enter y if yes, any other key if no!");
                var toContinue = Console.ReadLine();
                continueProgram = toContinue.ToLower() == "y";
            }
        }

        private static void ShowAdminMenu(HotelService hotelService)
        {
            var continueAdmin = true;
            while (continueAdmin)
            {
                Console.WriteLine("Please choose the following options:");
                Console.WriteLine(" 1.Add room\n 2.Edit room\n 3.Show active bookings\n 4.Show all rooms\n 5.Show all customers\n 6.Show booking by ID\n 7.Show customer by ID\n 8.Show room by ID");
                int.TryParse(Console.ReadLine(), out int userChoice);
                switch (userChoice)
                {
                    case 1:
                        hotelService.AddRoom();
                        break;
                    case 2:
                        hotelService.EditRoom();
                        break;
                    case 3:
                        hotelService.GetValidBookings();
                        break;
                    case 4:
                        hotelService.GetRooms();
                        break;
                    case 5:
                        hotelService.GetCustomers();
                        break;
                    case 6:
                        hotelService.GetBookingById();
                        break;
                    case 7:
                        hotelService.GetCustomerById();
                        break;
                    case 8:
                        hotelService.GetRoomById();
                        break;
                    default:
                        Console.WriteLine("Option not found!");
                        break;
                }
                Console.WriteLine("Do you want to continue admin menu?Enter y if yes, any other key if no!");
                var toContinue = Console.ReadLine();
                continueAdmin = toContinue.ToLower() == "y";
            }
        }
    }
}
