using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moto_List;
using Moto_List.Menu2;
using static Moto_List.Handle;
using static Moto_List.Log;
using static Moto_List.Menu2.List;
using static Moto_List.Menu2.Menu;

namespace Moto_List.Menu2
{
   /// <summary>
   /// Executes menu actions based on user selection.
   /// </summary>
   public class ExeMenuAction
   {
      private readonly IHandleOption _handle;
      private readonly Dictionary<MenuOption, bool> _selectionState;
      private readonly ILog _log;
      private readonly Dictionary<MenuOption, Func<Task>> _menuActions;
      private readonly AdminMenu _adminMenu;
      private readonly IAuthService _authService;

      /// <summary>
      /// Initializes a new instance of the <see cref="ExeMenuAction"/> class.
      /// </summary>
      public ExeMenuAction(IHandleOption handle, Dictionary<MenuOption, bool> selectionState, ILog log, IAuthService authService)
      {
         _handle = handle ?? throw new ArgumentNullException(nameof(handle));
         _selectionState = selectionState ?? throw new ArgumentNullException(nameof(selectionState));
         _log = log ?? throw new ArgumentNullException(nameof(log));
         _authService = authService ?? throw new ArgumentNullException(nameof(authService));
         _adminMenu = new AdminMenu(log);
         _menuActions = InitializeMenuActions();
      }

      private Dictionary<MenuOption, Func<Task>> InitializeMenuActions()
      {
         return new Dictionary<MenuOption, Func<Task>>
            {
                { MenuOption.GearBag, () => HandleOption(MenuOption.GearBag) },
                { MenuOption.Gloves, () => HandleOption(MenuOption.Gloves) },
                { MenuOption.ChestProtector, () => HandleOption(MenuOption.ChestProtector) },
                { MenuOption.NeckBrace, () => HandleOption(MenuOption.NeckBrace) },
                { MenuOption.HelmetBag, () => HandleOption(MenuOption.HelmetBag) },
                { MenuOption.Helmet, () => HandleOption(MenuOption.Helmet) },
                { MenuOption.GoggleBag, () => HandleOption(MenuOption.GoggleBag) },
                { MenuOption.Goggles, () => HandleOption(MenuOption.Goggles) },
                { MenuOption.GoggleReplacementLenses, () => HandleOption(MenuOption.GoggleReplacementLenses) },
                { MenuOption.GoggleTearOffs, () => HandleOption(MenuOption.GoggleTearOffs) },
                { MenuOption.GoggleRollOffs, () => HandleOption(MenuOption.GoggleRollOffs) },
                { MenuOption.GoggleAccessories, () => HandleOption(MenuOption.GoggleAccessories) },
                { MenuOption.KneeBraces, () => HandleOption(MenuOption.KneeBraces) },
                { MenuOption.KneeGuards, () => HandleOption(MenuOption.KneeGuards) },
                { MenuOption.ElbowGuards, () => HandleOption(MenuOption.ElbowGuards) },
                { MenuOption.KidneyBelt, () => HandleOption(MenuOption.KidneyBelt) },
                { MenuOption.RaceBoots, () => HandleOption(MenuOption.RaceBoots) },
                { MenuOption.AdminMenu, ShowAdminMenu },
                { MenuOption.LogOut, LogOut }
            };
      }

      /// <summary>
      /// Executes the selected menu action.
      /// </summary>
      public async Task ExecuteMenuAction(MenuOption option)
      {
         try
         {
            if (_menuActions.TryGetValue(option, out var action))
            {
               await action().ConfigureAwait(false);
            }
            else
            {
               await LogAndDisplayMessageAsync("Invalid option. Please try again.").ConfigureAwait(false);
            }
         }
         catch (Exception ex)
         {
            await LogAndDisplayMessageAsync($"An error occurred while executing the menu action: {ex.Message}").ConfigureAwait(false);
         }
      }

      private async Task HandleOption(MenuOption option)
      {
         try
         {
            await _handle.HandleOption(option, _selectionState).ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            await _log.LogSelectionAsync($"Error handling option {option}: {ex.Message}").ConfigureAwait(false);
            throw;
         }
      }

      private async Task ShowAdminMenu()
      {
         await LogAndDisplayMessageAsync("Entering Admin Menu...").ConfigureAwait(false);
         await _adminMenu.ShowAdminMenu(_selectionState).ConfigureAwait(false);
         await LogAndDisplayMessageAsync("Returning from Admin Menu...").ConfigureAwait(false);
         await MainMenu.ShowMainMenu().ConfigureAwait(false);
      }

      private async Task LogOut()
      {
         try
         {
            await LogAndDisplayMessageAsync("Logging out...").ConfigureAwait(false);
            await _log.LogSelectionAsync("User initiated log out.").ConfigureAwait(false);
            await LogToFileAsync("Logging out...").ConfigureAwait(false);

            await Task.Delay(1000).ConfigureAwait(false); // Optional: Add a delay to simulate logging out process

            bool isAuthenticated = await _authService.SetAuthenticationState(false).ConfigureAwait(false);

            if (isAuthenticated)
            {
               await MainMenu.ShowMainMenu().ConfigureAwait(false);
               await _log.LogSelectionAsync("User successfully logged out.").ConfigureAwait(false);
               await LogToFileAsync("User successfully logged out.").ConfigureAwait(false);
            }
            else
            {
               await LogAndDisplayMessageAsync("Authentication failed. Cannot log out.").ConfigureAwait(false);
            }
         }
         catch (Exception ex)
         {
            await LogAndDisplayMessageAsync($"An error occurred during log out: {ex.Message}").ConfigureAwait(false);
         }
      }

      private async Task LogToFileAsync(string message)
      {
         string logFilePath = "A:\\Moto_List\\logout_log.txt";
         using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
         {
            await writer.WriteLineAsync($"{DateTime.Now}: {message}").ConfigureAwait(false);
         }
      }

      private async Task LogAndDisplayMessageAsync(string message)
      {
         await Task.Run(() => Console.WriteLine(message)).ConfigureAwait(false);
         await _log.LogSelectionAsync(message).ConfigureAwait(false);
      }
   }
}
