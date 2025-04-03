using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Moto_List.Menu2.List;
using static Moto_List.Menu2.Menu;

namespace Moto_List.Menu2
{
   /// <summary>
   /// Represents the main menu and its actions.
   /// </summary>
   public class Menu : IExeMenuAction
   {
      private readonly Dictionary<MenuOption, bool> _selectionState;
      private readonly ILog _log;
      private readonly IHandle _handle;
      private readonly IMenuDisplay _menuDisplay;
      private readonly AdminMenu _adminMenu;

      /// <summary>
      /// Initializes a new instance of the <see cref="Menu"/> class.
      /// </summary>
      /// <param name="log">The log instance to use for logging.</param>
      /// <param name="handle">The handle instance to use for handling options.</param>
      /// <param name="menuDisplay">The menu display instance to use for displaying menu options.</param>
      /// <param name="adminMenu">The admin menu instance to use for admin actions.</param>
      public Menu(ILog log, IHandle handle, IMenuDisplay menuDisplay, AdminMenu adminMenu)
      {
         _log = log ?? throw new ArgumentNullException(nameof(log));
         _handle = handle ?? throw new ArgumentNullException(nameof(handle));
         _menuDisplay = menuDisplay ?? throw new ArgumentNullException(nameof(menuDisplay));
         _adminMenu = adminMenu ?? throw new ArgumentNullException(nameof(adminMenu));
         _selectionState = Enum.GetValues(typeof(MenuOption))
             .Cast<MenuOption>()
             .ToDictionary(option => option, option => false);
      }

      /// <summary>
      /// Displays the main menu and handles user input.
      /// </summary>
      public async Task ShowMenu()
      {
         while (true)
         {
            try
            {
               Console.Clear();
               _menuDisplay.DisplayMainMenuOptions(_selectionState);

               Console.Write("Please select an option: ");
               string input = Console.ReadLine();

               if (Enum.TryParse<MenuOption>(input, ignoreCase: true, out var selectedOption) && Enum.IsDefined(typeof(MenuOption), selectedOption))
               {
                  if (selectedOption == MenuOption.AdminMenu)
                  {
                     await _adminMenu.ShowAdminMenu(_selectionState).ConfigureAwait(false);
                  }
                  else
                  {
                     await ExecuteMenuAction(selectedOption).ConfigureAwait(false);
                  }
               }
               else
               {
                  ConsoleHelper.DisplayInvalidOptionMessage();
               }
            }
            catch (ArgumentNullException ex)
            {
               await LogAndDisplayErrorAsync($"Argument null error in ShowMenu: {ex.Message}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
               await LogAndDisplayErrorAsync($"An error occurred in ShowMenu: {ex.Message}").ConfigureAwait(false);
            }
         }
      }

      /// <summary>
      /// Executes the selected menu action.
      /// </summary>
      public async Task ExecuteMenuAction(MenuOption option)
      {
         try
         {
            switch (option)
            {
               case MenuOption.LogOut:
                  await _log.LogSelectionAsync("User logged out.").ConfigureAwait(false);
                  await MainMenu.ShowMainMenu().ConfigureAwait(false); // Call ShowMainMenu after logout
                  break;
               default:
                  await _handle.HandleOption(option, _selectionState).ConfigureAwait(false);
                  break;
            }

            await _log.LogSelectionAsync($"Selected option: {option}").ConfigureAwait(false);
         }
         catch (ArgumentNullException ex)
         {
            await LogAndDisplayErrorAsync($"Argument null error while executing menu action: {ex.Message}").ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            await LogAndDisplayErrorAsync($"An error occurred while executing menu action: {ex.Message}").ConfigureAwait(false);
         }
      }

      private async Task LogAndDisplayErrorAsync(string message)
      {
         await _log.LogSelectionAsync(message).ConfigureAwait(false);
         ConsoleHelper.DisplayErrorMessage();
      }

      /// <summary>
      /// Interface for executing menu actions.
      /// </summary>
      public interface IExeMenuAction
      {
         /// <summary>
         /// Executes the selected menu action.
         /// </summary>
         Task ExecuteMenuAction(MenuOption option);
      }

      /// <summary>
      /// Interface for logging.
      /// </summary>
      public interface ILog
      {
         /// <summary>
         /// Logs a selection message asynchronously.
         /// </summary>
         Task LogSelectionAsync(string message);
      }

      /// <summary>
      /// Interface for handling menu options.
      /// </summary>
      public interface IHandle
      {
         /// <summary>
         /// Handles the specified menu option and updates the selection state.
         /// </summary>
         Task HandleOption(MenuOption option, Dictionary<MenuOption, bool> selectionState);
      }

      /// <summary>
      /// Interface for displaying menu options.
      /// </summary>
      public interface IMenuDisplay
      {
         /// <summary>
         /// Displays the main menu options.
         /// </summary>
         void DisplayMainMenuOptions(Dictionary<MenuOption, bool> selectionState);

         /// <summary>
         /// Displays the admin and logout options.
         /// </summary>
         void DisplayAdminLogoutOptions();
      }
   }
}
