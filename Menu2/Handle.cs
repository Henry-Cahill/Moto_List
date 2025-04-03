using Moto_List.Menu2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Moto_List.Menu2.List;
using static Moto_List.Menu2.Menu;

namespace Moto_List
{
   /// <summary>
   /// Handles menu options and their selection states.
   /// </summary>
   public class Handle : IHandle
   {
      private readonly ILog _log;

      /// <summary>
      /// Initializes a new instance of the <see cref="Handle"/> class.
      /// </summary>
      /// <param name="log">The log instance to use for logging.</param>
      public Handle(ILog log)
      {
         _log = log ?? throw new ArgumentNullException(nameof(log));
      }

      /// <summary>
      /// Handles the specified menu option and updates the selection state.
      /// </summary>
      /// <param name="option">The menu option to handle.</param>
      /// <param name="selectionState">The selection state dictionary.</param>
      public async Task HandleOption(MenuOption option, Dictionary<MenuOption, bool> selectionState)
      {
         if (selectionState == null)
         {
            throw new ArgumentNullException(nameof(selectionState));
         }

         try
         {
            // Simulate async work
            await Task.Delay(100).ConfigureAwait(false);
            selectionState[option] = !selectionState[option];
            await _log.LogSelectionAsync($"Option {option} handled.").ConfigureAwait(false);
         }
         catch (Exception ex)
         {
            await _log.LogSelectionAsync($"Error handling option {option}: {ex.Message}").ConfigureAwait(false);
            throw;
         }
      }

      /// <summary>
      /// Interface for handling menu options.
      /// </summary>
      public interface IHandleOption
      {
         /// <summary>
         /// Handles the specified menu option and updates the selection state.
         /// </summary>
         /// <param name="option">The menu option to handle.</param>
         /// <param name="selectionState">The selection state dictionary.</param>
         Task HandleOption(MenuOption option, Dictionary<MenuOption, bool> selectionState);
      }
   }
}
