using System;
using System.Collections.Generic;
using System.Linq;

namespace Moto_List
{
   public static class DisplayMenuOptions
   {
      public static void Show(Dictionary<Menu.MenuOption, bool> selectionState)
      {
         var header = "MOTOCROSS READY – CHECKLIST";
         var subHeader = "Ride Hard, Ride Safe!";
         var riderGearHeader = "**Rider Gear**";
         var bikeCheckHeader = "[✓] **Bike Pre-Ride Check**";
         var bikeCheckItems = new List<string>
            {
                "Tire Pressure & Tread",
                "Chain & Sprocket Condition",
                "Brakes & Suspension",
                "Fluid Levels (Oil, Coolant)",
                "Battery & Electrical"
            };

         var menuOptions = Enum.GetValues(typeof(Menu.MenuOption)).Cast<Menu.MenuOption>().ToList();
         var longestLine = Math.Max(menuOptions.Max(option => option.ToString().Length + 8), bikeCheckItems.Max(item => item.Length) + 4);
         var boxWidth = Math.Max(longestLine, header.Length) + 4;

         PrintLine(boxWidth);
         PrintCenteredText(header, boxWidth);
         PrintCenteredText(subHeader, boxWidth);
         PrintEmptyLine(boxWidth);

         PrintCenteredText(riderGearHeader, boxWidth);
         foreach (var option in menuOptions)
         {
            string status = selectionState[option] ? "[✓]" : "[ ]";
            Console.WriteLine($"│ {status} {(int)option}. {option,-{boxWidth - 10}} │");
         }
         PrintEmptyLine(boxWidth);

         PrintCenteredText(bikeCheckHeader, boxWidth);
         foreach (var item in bikeCheckItems)
         {
            Console.WriteLine($"│   • {item,-{boxWidth - 6}} │");
         }
         PrintEmptyLine(boxWidth);
         PrintLine(boxWidth);
      }

      private static void PrintLine(int width)
      {
         Console.WriteLine("┌" + new string('─', width - 2) + "┐");
      }

      private static void PrintEmptyLine(int width)
      {
         Console.WriteLine("│" + new string(' ', width - 2) + "│");
      }

      private static void PrintCenteredText(string text, int width)
      {
         int padding = (width - text.Length - 2) / 2;
         Console.WriteLine("│" + new string(' ', padding) + text + new string(' ', width - text.Length - padding - 2) + "│");
      }
   }
}
