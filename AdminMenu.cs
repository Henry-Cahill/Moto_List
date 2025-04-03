using Moto_List.Menu2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Moto_List.Menu2.List;
using static Moto_List.Menu2.Menu;

namespace Moto_List
{
   /// <summary>
   /// Represents the admin menu and its actions.
   /// </summary>
   public class AdminMenu
   {
      private readonly ILog _log;

      /// <summary>
      /// Initializes a new instance of the <see cref="AdminMenu"/> class.
      /// </summary>
      /// <param name="log">The log instance to use for logging.</param>
      public AdminMenu(ILog log)
      {
         _log = log ?? throw new ArgumentNullException(nameof(log));
      }

      /// <summary>
      /// Enum representing the available admin options.
      /// </summary>
      public enum AdminOption
      {
         SelectAll,
         UnselectAll,
         Back
      }

      /// <summary>
      /// Displays the admin menu and handles user input.
      /// </summary>
      /// <param name="selectionState">The selection state dictionary.</param>
      public async Task ShowAdminMenu(Dictionary<MenuOption, bool> selectionState)
      {
         if (selectionState == null)
         {
            throw new ArgumentNullException(nameof(selectionState));
         }

         while (true)
         {
            Console.Clear();
            DisplayAdminOptions();

            Console.Write("Please choose an option from the Admin Menu: ");
            string input = Console.ReadLine();

            if (Enum.TryParse(input, ignoreCase: true, out AdminOption selectedOption) && Enum.IsDefined(typeof(AdminOption), selectedOption))
            {
               if (await ExecuteAdminAction(selectedOption, selectionState).ConfigureAwait(false))
               {
                  break;
               }
            }
            else
            {
               await _log.LogSelectionAsync($"Invalid admin option selected: {input}").ConfigureAwait(false);
               ConsoleHelper.DisplayInvalidOptionMessage();
            }
         }
      }

      private void DisplayAdminOptions()
      {
         Console.WriteLine("=== Admin Menu ===");
         foreach (AdminOption option in Enum.GetValues(typeof(AdminOption)))
         {
            Console.WriteLine($"[{(int)option}] {option}");
         }
      }

      private async Task<bool> ExecuteAdminAction(AdminOption option, Dictionary<MenuOption, bool> selectionState)
      {
         switch (option)
         {
            case AdminOption.SelectAll:
               SelectAllOptions(selectionState);
               break;
            case AdminOption.UnselectAll:
               UnselectAllOptions(selectionState);
               break;
            case AdminOption.Back:
               return true;
            default:
               ConsoleHelper.DisplayInvalidOptionMessage();
               break;
         }

         await _log.LogSelectionAsync($"Admin action executed: {option}").ConfigureAwait(false);
         ConsoleHelper.DisplayMessageAndWait("Press any key to continue...");
         return false;
      }

      private void SelectAllOptions(Dictionary<MenuOption, bool> selectionState)
      {
         foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
         {
            selectionState[option] = true;
         }
         Console.WriteLine("All options have been selected.");
      }

      private void UnselectAllOptions(Dictionary<MenuOption, bool> selectionState)
      {
         foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
         {
            selectionState[option] = false;
         }
         Console.WriteLine("All options have been unselected.");
      }
   }
}
