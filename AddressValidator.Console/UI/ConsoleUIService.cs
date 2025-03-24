using Spectre.Console;
using System;
using AddressValidator.Console.Models;

namespace AddressValidator.Console.UI
{
    public class ConsoleUIService
    {
        /// <summary>
        /// Clears the console and prevents scrolling to previous content
        /// </summary>
        public void Initialize()
        {
            // Clear the console completely
            AnsiConsole.Clear();
            
            // On Windows, we can resize the buffer to match the window size
            // which effectively prevents scrolling up to history
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    System.Console.SetBufferSize(System.Console.WindowWidth, System.Console.WindowHeight);
                    
                    // Set console window position to center on screen (Windows only)
                    System.Console.SetWindowPosition(
                        (System.Console.LargestWindowWidth - System.Console.WindowWidth) / 2,
                        (System.Console.LargestWindowHeight - System.Console.WindowHeight) / 2);
                }
                catch (Exception)
                {
                    // Some terminals don't support these operations
                    // Just continue if this fails
                }
            }
            
            // Show welcome screen
            ShowWelcomeScreen();
        }
        
        /// <summary>
        /// Displays a welcome splash screen
        /// </summary>
        public void ShowWelcomeScreen()
        {
            AnsiConsole.Write(new FigletText("Address Validator")
                .Centered()
                .Color(Color.Blue));
                
            AnsiConsole.Write(new Rule("[blue]Azure Maps Validation Tool[/]").Centered());
            
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            System.Console.ReadKey(true);
            AnsiConsole.Clear();
        }

        /// <summary>
        /// Shows the main menu and returns the selected option
        /// </summary>
        public string ShowMainMenu()
        {
            AnsiConsole.Clear();
            
            // Application title
            AnsiConsole.Write(new FigletText("Address Validator")
                .Centered()
                .Color(Color.Blue));
            
            AnsiConsole.Write(new Rule("[blue]Main Menu[/]").Centered());
            
            // Display the menu using Spectre.Console's SelectionPrompt
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\n[yellow]What would you like to do?[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .AddChoices(new[] 
                    {
                        "1. Validate new address",
                        "2. View validation history",
                        "3. Re-validate address from history",
                        "4. Clear validation history",
                        "5. Exit application"
                    }));
            
            return option;
        }
        
        /// <summary>
        /// Shows a progress spinner while an operation is in progress
        /// Blocks all user input until the operation completes
        /// </summary>
        public async Task<T> ShowSpinnerAsync<T>(string message, Func<Task<T>> action)
        {
            // Clear the screen
            AnsiConsole.Clear();
            
            // No vertical spacing or centering - let the spinner appear at the top left
            
            return await AnsiConsole.Status()
                .AutoRefresh(true)
                .SpinnerStyle(new Style(foreground: Color.Green))
                .Spinner(Spinner.Known.Dots)
                .StartAsync(message, async ctx =>
                {
                    // Ensure UI is refreshed to prevent input
                    ctx.Refresh();
                    
                    // Execute the action while effectively blocking input
                    return await action();
                });
        }
        
        /// <summary>
        /// Shows a "Press any key to continue" prompt
        /// </summary>
        public void PressAnyKeyToContinue()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule());
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
            System.Console.ReadKey(true);
        }
        
        /// <summary>
        /// Shows a goodbye message when exiting the application
        /// </summary>
        public void ShowGoodbye()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Panel(
                Align.Center(
                    new Markup("[blue]Thank you for using Address Validator![/]\n\n[grey]Application developed with Azure Maps and Spectre.Console[/]")
                )
            )
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 1),
                BorderStyle = new Style(foreground: Color.Blue)
            });
            
            AnsiConsole.WriteLine();
        }
        
        /// <summary>
        /// Shows an error message
        /// </summary>
        public void ShowError(string message)
        {
            AnsiConsole.Write(new Panel(message)
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(foreground: Color.Red),
                Padding = new Padding(1, 0)
            });
        }
    }
}