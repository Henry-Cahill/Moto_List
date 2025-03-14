// See https://aka.ms/new-console-template for more information
using System;

namespace Moto_List
{
   class MainMenu
   {
      static void Main(string[] args)
      {
         ShowMainMenu();
      }

      public static void ShowMainMenu()
      {
         Console.Clear();
         Console.WriteLine("Welcome to the Application");
         Console.WriteLine("1. Login");
         Console.WriteLine("2. Exit");
         Console.Write("Please select an option: ");

         switch (Console.ReadLine())
         {
            case "1":
               Login();
               break;
            case "2":
               Environment.Exit(0);
               break;
            default:
               Console.WriteLine("Invalid option. Please try again.");
               ShowMainMenu();
               break;
         }
      }

      static void Login()
      {
         Console.Clear();
         Console.Write("Enter username: ");
         string username = Console.ReadLine();
         Console.Write("Enter password: ");
         string password = Console.ReadLine();

         // For simplicity, using hardcoded credentials
         if (username == "admin" && password == "password")
         {
            Console.WriteLine("Login successful!");
            // Proceed to the next part of the application
            Menu menu = new Menu();
            menu.ShowMenu();
         }
         else
         {
            Console.WriteLine("Invalid credentials. Please try again.");
            ShowMainMenu();
         }
      }
   }
}
