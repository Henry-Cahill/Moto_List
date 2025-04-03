using System;

namespace Moto_List
{
   /// <summary>
   /// Provides helper methods for console operations.
   /// </summary>
   public static class ConsoleHelper
   {
      /// <summary>
      /// Displays an invalid option message to the console.
      /// </summary>
      public static void DisplayInvalidOptionMessage()
      {
         Console.WriteLine("Invalid option. Please try again.");
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
      }

      /// <summary>
      /// Displays a generic error message to the console.
      /// </summary>
      public static void DisplayErrorMessage()
      {
         Console.WriteLine("An unexpected error occurred. Please try again.");
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
      }

      /// <summary>
      /// Displays a custom message to the console and waits for a key press.
      /// </summary>
      /// <param name="message">The message to display.</param>
      public static void DisplayMessageAndWait(string message)
      {
         Console.WriteLine(message);
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
      }
   }
}