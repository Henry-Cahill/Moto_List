using Moto_List.Menu2;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using static Moto_List.Menu2.Menu;

namespace Moto_List
{
   /// <summary>
   /// Represents the main menu and its actions.
   /// </summary>
   class MainMenu
   {
      private static IServiceProvider _serviceProvider;

      /// <summary>
      /// The main entry point of the application.
      /// </summary>
      static async Task Main(string[] args)
      {
         ConfigureServices();
         await ShowMainMenu().ConfigureAwait(false);
      }

      /// <summary>
      /// Configures the services for dependency injection.
      /// </summary>
      private static void ConfigureServices()
      {
         var serviceCollection = new ServiceCollection();
         serviceCollection.AddSingleton<ILog, Log>();
         serviceCollection.AddSingleton<IHandle, Handle>();
         serviceCollection.AddSingleton<IMenuDisplay, MenuDisplay>();
         serviceCollection.AddSingleton<Menu>();
         serviceCollection.AddSingleton<IAuthService, AuthService>();
         serviceCollection.AddSingleton<AdminMenu>();
         serviceCollection.AddSingleton<IConfiguration>(provider => new ConfigurationBuilder().AddJsonFile("A:\\Moto_List\\appsettings.json").Build());

         _serviceProvider = serviceCollection.BuildServiceProvider();
      }

      /// <summary>
      /// Displays the main menu and handles user input.
      /// </summary>
      public static async Task ShowMainMenu()
      {
         var log = _serviceProvider.GetService<ILog>();
         var menu = _serviceProvider.GetService<Menu>();

         while (true)
         {
            try
            {
               DisplayMainMenu();
               string input = GetUserInput();

               if (string.IsNullOrWhiteSpace(input))
               {
                  ConsoleHelper.DisplayInvalidOptionMessage();
                  continue;
               }

               await HandleMenuSelection(input, menu).ConfigureAwait(false);
            }
            catch (ArgumentNullException ex)
            {
               await HandleMenuError(log, ex, "Argument null error in ShowMainMenu").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
               await HandleMenuError(log, ex, "An error occurred in ShowMainMenu").ConfigureAwait(false);
            }
         }
      }

      private static void DisplayMainMenu()
      {
         Console.Clear();
         Console.WriteLine("Welcome to the Application!");
         Console.WriteLine();
         Console.WriteLine("Please choose an option:");
         Console.WriteLine("1. Login");
         Console.WriteLine("2. Exit");
         Console.Write("Enter your selection: ");
      }

      private static string GetUserInput()
      {
         return Console.ReadLine();
      }

      private static async Task HandleMenuSelection(string input, Menu menu)
      {
         switch (input)
         {
            case "1":
            case "Login":
               await Login(menu).ConfigureAwait(false);
               break;
            case "2":
            case "Exit":
               Environment.Exit(0);
               break;
            default:
               ConsoleHelper.DisplayInvalidOptionMessage();
               break;
         }
      }

      private static async Task HandleMenuError(ILog log, Exception ex, string message)
      {
         await log.LogSelectionAsync($"{message}: {ex.Message}").ConfigureAwait(false);
         ConsoleHelper.DisplayErrorMessage();
      }

      private static async Task Login(Menu menu)
      {
         var log = _serviceProvider.GetService<ILog>();
         var authService = _serviceProvider.GetService<IAuthService>();
         var adminMenu = _serviceProvider.GetService<AdminMenu>();

         try
         {
            Console.Clear();
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadPassword();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
               ConsoleHelper.DisplayMessageAndWait("Username and password cannot be empty. Please try again.");
               return;
            }

            if (await authService.Authenticate(username, password).ConfigureAwait(false))
            {
               Console.WriteLine("Login successful!");
               await log.LogSelectionAsync("User logged in successfully.").ConfigureAwait(false);
               await menu.ShowMenu().ConfigureAwait(false);
               await adminMenu.ShowAdminMenu(new Dictionary<List.MenuOption, bool>()).ConfigureAwait(false);
               await ShowMainMenu().ConfigureAwait(false); // Add this line to return to the main menu
            }
            else
            {
               ConsoleHelper.DisplayMessageAndWait("Invalid credentials. Please try again.");
            }
         }
         catch (ArgumentNullException ex)
         {
            await log.LogSelectionAsync($"Argument null error during login: {ex.Message}").ConfigureAwait(false);
            ConsoleHelper.DisplayErrorMessage();
         }
         catch (Exception ex)
         {
            await log.LogSelectionAsync($"An error occurred during login: {ex.Message}").ConfigureAwait(false);
            ConsoleHelper.DisplayErrorMessage();
         }
      }

      private static string ReadPassword()
      {
         string password = string.Empty;
         ConsoleKey key;
         do
         {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
               Console.Write("\b \b");
               password = password[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
               Console.Write("*");
               password += keyInfo.KeyChar;
            }
         } while (key != ConsoleKey.Enter);
         Console.WriteLine();
         return password;
      }
   }
}