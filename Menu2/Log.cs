using System;
using System.Threading.Tasks;
using static Moto_List.Menu2.Menu;

namespace Moto_List
{
   /// <summary>
   /// Provides logging functionality.
   /// </summary>
   public class Log : ILog
   {
      /// <summary>
      /// Logs a selection message asynchronously.
      /// </summary>
      /// <param name="message">The message to log.</param>
      public async Task LogSelectionAsync(string message)
      {
         if (string.IsNullOrWhiteSpace(message))
         {
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
         }

         // Simulate logging logic
         await Task.Delay(50).ConfigureAwait(false); // Simulate async work
         Console.WriteLine($"Log: {message}");
      }

      /// <summary>
      /// Interface for selection state.
      /// </summary>
      public interface ISelectionState
      {
         // Define the members of the ISelectionState interface here
      }
   }
}
