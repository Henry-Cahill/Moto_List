using System;
using System.IO;
using System.Threading.Tasks;

namespace Moto_List
{
   /// <summary>
   /// Interface for authentication service.
   /// </summary>
   public interface IAuthService
   {
      /// <summary>
      /// Authenticates the user with the given username and password.
      /// </summary>
      Task<bool> Authenticate(string username, string password);

      /// <summary>
      /// Sets the authentication state.
      /// </summary>
      Task<bool> SetAuthenticationState(bool isAuthenticated);

      /// <summary>
      /// Logs out the user.
      /// </summary>
      Task LogOut();
   }

   /// <summary>
   /// Provides authentication services.
   /// </summary>
   public class AuthService : IAuthService
   {
      private bool _isAuthenticated;

      /// <summary>
      /// Authenticates the user with the given username and password.
      /// </summary>
      public async Task<bool> Authenticate(string username, string password)
      {
         // Simulate authentication logic
         await Task.Delay(100).ConfigureAwait(false); // Simulate async work
         _isAuthenticated = username == "admin" && password == "password";
         return _isAuthenticated;
      }

      /// <summary>
      /// Sets the authentication state.
      /// </summary>
      public async Task<bool> SetAuthenticationState(bool isAuthenticated)
      {
         await Task.Delay(50).ConfigureAwait(false); // Simulate async work
         _isAuthenticated = isAuthenticated;
         return _isAuthenticated;
      }

      /// <summary>
      /// Logs out the user.
      /// </summary>
      public async Task LogOut()
      {
         try
         {
            await LogAndDisplayMessageAsync("Logging out...").ConfigureAwait(false);

            await Task.Delay(1000).ConfigureAwait(false); // Optional: Add a delay to simulate logging out process

            // Force re-authentication by setting the authentication value to false
            bool isAuthenticated = await SetAuthenticationState(false).ConfigureAwait(false);

            if (isAuthenticated)
            {
               await LogAndDisplayMessageAsync("User successfully logged out.").ConfigureAwait(false);
               await MainMenu.ShowMainMenu().ConfigureAwait(false); // Add this line to return to the main menu
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
         await LogToFileAsync(message).ConfigureAwait(false);
      }
   }
}

