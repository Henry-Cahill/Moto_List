using Moto_List.Menu2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Moto_List
{
   /// <summary>
   /// Provides functionality to display menu options.
   /// </summary>
   public static class DisplayMenuOptions
   {
      private const string Header = "MOTOCROSS READY – CHECKLIST";
      private const string SubHeader = "Ride Hard, Ride Safe!";
      private const string RiderGearHeader = "**Rider Gear**";
      private const string BikeCheckHeader = "[✓] **Bike Pre-Ride Check**";
      private static readonly List<string> BikeCheckItems = new List<string>
        {
            "Tire Pressure & Tread",
            "Chain & Sprocket Condition",
            "Brakes & Suspension",
            "Fluid Levels (Oil, Coolant)",
            "Battery & Electrical"
        };

      /// <summary>
      /// Displays the menu options to a file.
      /// </summary>
      /// <param name="selectionState">The selection state dictionary.</param>
      /// <param name="filePath">The file path to write the menu options to.</param>
      public static void Show(Dictionary<List.MenuOption, bool> selectionState, string filePath)
      {
         if (selectionState == null)
         {
            throw new ArgumentNullException(nameof(selectionState));
         }

         if (string.IsNullOrWhiteSpace(filePath))
         {
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
         }

         using (StreamWriter writer = new StreamWriter(filePath))
         {
            var menuOptions = Enum.GetValues(typeof(List.MenuOption)).Cast<List.MenuOption>().ToList();
            var longestLine = Math.Max(menuOptions.Max(option => option.ToString().Length + 8), BikeCheckItems.Max(item => item.Length) + 4);
            var boxWidth = Math.Max(longestLine, Header.Length) + 4;

            PrintLine(writer, boxWidth);
            PrintCenteredText(writer, Header, boxWidth);
            PrintCenteredText(writer, SubHeader, boxWidth);
            PrintEmptyLine(writer, boxWidth);

            PrintCenteredText(writer, RiderGearHeader, boxWidth);
            foreach (var option in selectionState.Keys)
            {
               string status = selectionState[option] ? "[✓]" : "[ ]";
               string optionText = $"{status} {(int)option}. {option}";
               PrintAlignedText(writer, optionText, boxWidth);
            }

            PrintEmptyLine(writer, boxWidth);

            PrintCenteredText(writer, BikeCheckHeader, boxWidth);
            foreach (var item in BikeCheckItems)
            {
               PrintAlignedText(writer, $"• {item}", boxWidth);
            }
            PrintEmptyLine(writer, boxWidth);
            PrintLine(writer, boxWidth);
         }
      }

      private static void PrintLine(StreamWriter writer, int width)
      {
         writer.WriteLine("┌" + new string('─', width - 2) + "┐");
      }

      private static void PrintEmptyLine(StreamWriter writer, int width)
      {
         writer.WriteLine("│" + new string(' ', width - 2) + "│");
      }

      private static void PrintCenteredText(StreamWriter writer, string text, int width)
      {
         int padding = (width - text.Length - 2) / 2;
         writer.WriteLine("│" + new string(' ', padding) + text + new string(' ', width - text.Length - padding - 2) + "│");
      }

      private static void PrintAlignedText(StreamWriter writer, string text, int width)
      {
         int padding = width - text.Length - 2;
         writer.WriteLine("│ " + text + new string(' ', padding) + "│");
      }
   }
}