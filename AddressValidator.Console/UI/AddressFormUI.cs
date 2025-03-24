using Spectre.Console;
using AddressValidator.Console.Models;
using System.Collections.Generic;

namespace AddressValidator.Console.UI
{
    public class AddressFormUI
    {
        /// <summary>
        /// Collect address details from the user with field validation and review capability
        /// </summary>
        public static AddressInput? CollectAddressInput()
        {
            AnsiConsole.Clear();
            
            // Add a title with styling
            AnsiConsole.Write(new FigletText("Address Form")
                .Centered()
                .Color(Color.Blue));
                
            AnsiConsole.Write(new Rule("[blue]Enter Address Details[/]").Centered());
            AnsiConsole.WriteLine();
            
            // Ask if the user wants to proceed or go back
            var proceedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Would you like to proceed with address validation?[/]")
                    .PageSize(3)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .AddChoices(new[] {
                        "Proceed with validation",
                        "Return to main menu"
                    })
            );
            
            if (proceedOption == "Return to main menu")
            {
                return null;
            }
            
            try
            {
                // Initialize the address with empty values
                var addressInput = new AddressInput
                {
                    AddressLine1 = "",
                    AddressLine2 = "",
                    AddressLine3 = "",
                    PostalCode = "",
                    City = "",
                    Country = ""
                };
                
                bool isReviewing = false;
                bool isComplete = false;
                
                while (!isComplete)
                {
                    // If we're in review mode, show the current values and options to edit
                    if (isReviewing)
                    {
                        // Clear the console for the review screen
                        AnsiConsole.Clear();
                        
                        // Re-display the title
                        AnsiConsole.Write(new FigletText("Address Form")
                            .Centered()
                            .Color(Color.Blue));
                        
                        AnsiConsole.Write(new Rule("[blue]Review Address Details[/]").Centered());
                        AnsiConsole.WriteLine();
                        
                        // Create options array with current values embedded
                        var choices = new List<string>
                        {
                            "Submit for validation"
                        };
                        
                        // Add editing options with current values
                        choices.Add($"Edit Address Line 1: {FormatValueForDisplay(addressInput.AddressLine1)}");
                        choices.Add($"Edit Address Line 2: {FormatValueForDisplay(addressInput.AddressLine2)}");
                        choices.Add($"Edit Address Line 3: {FormatValueForDisplay(addressInput.AddressLine3)}");
                        choices.Add($"Edit Postal Code: {FormatValueForDisplay(addressInput.PostalCode)}");
                        choices.Add($"Edit City: {FormatValueForDisplay(addressInput.City)}");
                        choices.Add($"Edit Country: {FormatValueForDisplay(addressInput.Country)}");
                        choices.Add("Cancel and return to main menu");
                        
                        // Present options for editing or submitting
                        var reviewOption = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[yellow]What would you like to do?[/]")
                                .PageSize(8)
                                .HighlightStyle(new Style(foreground: Color.Blue))
                                .AddChoices(choices)
                        );
                        
                        // Handle the selected option
                        if (reviewOption == "Submit for validation")
                        {
                            // Proceed directly to validation without the additional confirmation screen
                            isComplete = true;
                        }
                        else if (reviewOption == "Cancel and return to main menu")
                        {
                            return null;
                        }
                        else if (reviewOption.StartsWith("Edit Address Line 1:"))
                        {
                            addressInput.AddressLine1 = EditField("Address Line 1", addressInput.AddressLine1, true);
                            if (addressInput.AddressLine1 == "!cancel")
                                return null;
                        }
                        else if (reviewOption.StartsWith("Edit Address Line 2:"))
                        {
                            addressInput.AddressLine2 = EditField("Address Line 2", addressInput.AddressLine2, false);
                            if (addressInput.AddressLine2 == "!cancel")
                                return null;
                        }
                        else if (reviewOption.StartsWith("Edit Address Line 3:"))
                        {
                            addressInput.AddressLine3 = EditField("Address Line 3", addressInput.AddressLine3, false);
                            if (addressInput.AddressLine3 == "!cancel")
                                return null;
                        }
                        else if (reviewOption.StartsWith("Edit Postal Code:"))
                        {
                            addressInput.PostalCode = EditField("Postal Code", addressInput.PostalCode, true);
                            if (addressInput.PostalCode == "!cancel")
                                return null;
                        }
                        else if (reviewOption.StartsWith("Edit City:"))
                        {
                            addressInput.City = EditField("City", addressInput.City, true);
                            if (addressInput.City == "!cancel")
                                return null;
                        }
                        else if (reviewOption.StartsWith("Edit Country:"))
                        {
                            addressInput.Country = EditField("Country", addressInput.Country, true);
                            if (addressInput.Country == "!cancel")
                                return null;
                        }
                    }
                    else
                    {
                        // First time collection of all fields
                        AnsiConsole.Clear();
                        
                        // Re-display the title
                        AnsiConsole.Write(new FigletText("Address Form")
                            .Centered()
                            .Color(Color.Blue));
                            
                        AnsiConsole.Write(new Rule("[blue]Enter Address Details[/]").Centered());
                        AnsiConsole.WriteLine();
                        
                        // Create a centered form layout using a Panel
                        var infoPanel = new Panel(
                            new Markup("[yellow]Please fill in the address information below[/]\n[grey](Fields marked in green are required)[/]")
                        )
                        {
                            Border = BoxBorder.Rounded,
                            Padding = new Padding(1, 1),
                            Expand = false,
                            BorderStyle = Style.Parse("blue"),
                        };
                        
                        // Center the panel in the console
                        AnsiConsole.Write(Align.Center(infoPanel));
                        
                        AnsiConsole.WriteLine();
                        
                        // Show cancel instructions
                        AnsiConsole.MarkupLine("\n[grey]Type [yellow]!cancel[/] at any prompt to return to main menu[/]\n");
                        
                        // Collect all the input fields
                        
                        // Address Line 1 (required)
                        addressInput.AddressLine1 = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]Address Line 1[/] (required): ")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Address Line 1 is required[/]")
                                .Validate(address => 
                                    address.Trim() == "!cancel"
                                        ? ValidationResult.Success()
                                        : !string.IsNullOrWhiteSpace(address) 
                                            ? ValidationResult.Success() 
                                            : ValidationResult.Error("[red]Address Line 1 cannot be empty[/]")));
                                            
                        // Check for cancellation
                        if (addressInput.AddressLine1.Trim() == "!cancel")
                            return null;
                        
                        // Address Line 2 (optional)
                        addressInput.AddressLine2 = AnsiConsole.Prompt(
                            new TextPrompt<string>("[grey]Address Line 2[/] (optional): ")
                                .AllowEmpty());
                                
                        // Check for cancellation
                        if (addressInput.AddressLine2.Trim() == "!cancel")
                            return null;
                        
                        // Address Line 3 (optional)
                        addressInput.AddressLine3 = AnsiConsole.Prompt(
                            new TextPrompt<string>("[grey]Address Line 3[/] (optional): ")
                                .AllowEmpty());
                                
                        // Check for cancellation
                        if (addressInput.AddressLine3.Trim() == "!cancel")
                            return null;
                        
                        // Postal Code (required)
                        addressInput.PostalCode = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]Postal Code/ZIP[/] (required): ")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Postal Code is required[/]")
                                .Validate(code => 
                                    code.Trim() == "!cancel" 
                                        ? ValidationResult.Success()
                                        : !string.IsNullOrWhiteSpace(code) 
                                            ? ValidationResult.Success() 
                                            : ValidationResult.Error("[red]Postal Code cannot be empty[/]")));
                                            
                        // Check for cancellation
                        if (addressInput.PostalCode.Trim() == "!cancel")
                            return null;
                        
                        // City (required)
                        addressInput.City = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]City[/] (required): ")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]City is required[/]")
                                .Validate(c => 
                                    c.Trim() == "!cancel"
                                        ? ValidationResult.Success()
                                        : !string.IsNullOrWhiteSpace(c) 
                                            ? ValidationResult.Success() 
                                            : ValidationResult.Error("[red]City cannot be empty[/]")));
                                            
                        // Check for cancellation
                        if (addressInput.City.Trim() == "!cancel")
                            return null;
                        
                        // Country (required)
                        addressInput.Country = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]Country[/] (required): ")
                                .PromptStyle("green")
                                .ValidationErrorMessage("[red]Country is required[/]")
                                .Validate(c => 
                                    c.Trim() == "!cancel"
                                        ? ValidationResult.Success()
                                        : !string.IsNullOrWhiteSpace(c) 
                                            ? ValidationResult.Success() 
                                            : ValidationResult.Error("[red]Country cannot be empty[/]")));
                                            
                        // Check for cancellation
                        if (addressInput.Country.Trim() == "!cancel")
                            return null;
                            
                        // Switch to review mode after collecting all fields
                        isReviewing = true;
                    }
                }
                
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule());
                
                return addressInput;
            }
            catch (Exception ex)
            {
                // If any error occurs during input, treat it as a cancellation
                AnsiConsole.MarkupLine("[yellow]Input cancelled.[/]");
                return null;
            }
        }
        
        /// <summary>
        /// Format a value for display in the menu, showing empty values clearly
        /// </summary>
        private static string FormatValueForDisplay(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "[dim]<empty>[/]" : $"[bold]{value}[/]";
        }
        
        /// <summary>
        /// Formats the address details for display in the confirmation panel
        /// </summary>
        private static string GetAddressDetailsMarkup(AddressInput address)
        {
            var lines = new List<string>();
            
            // Add each field with proper formatting and colors
            lines.Add($"[green bold]Address Line 1:[/] [white]{address.AddressLine1}[/]");
            
            if (!string.IsNullOrWhiteSpace(address.AddressLine2))
                lines.Add($"[grey]Address Line 2:[/] [white]{address.AddressLine2}[/]");
            else
                lines.Add($"[grey]Address Line 2:[/] [dim]<empty>[/]");
                
            if (!string.IsNullOrWhiteSpace(address.AddressLine3))
                lines.Add($"[grey]Address Line 3:[/] [white]{address.AddressLine3}[/]");
            else
                lines.Add($"[grey]Address Line 3:[/] [dim]<empty>[/]");
                
            lines.Add($"[green bold]Postal Code:[/] [white]{address.PostalCode}[/]");
            lines.Add($"[green bold]City:[/] [white]{address.City}[/]");
            lines.Add($"[green bold]Country:[/] [white]{address.Country}[/]");
            
            // Add a separator and single-line format for reference
            lines.Add("");
            lines.Add("[yellow]Single-line format:[/]");
            lines.Add($"[white]{address.ToSingleLineString()}[/]");
            
            return string.Join("\n", lines);
        }
        
        /// <summary>
        /// Allow editing of a specific field, showing the current value
        /// </summary>
        private static string EditField(string fieldName, string currentValue, bool isRequired)
        {
            AnsiConsole.Clear();
            
            // Show a title for the edit screen
            AnsiConsole.Write(new Rule($"[blue]Edit {fieldName}[/]").Centered());
            AnsiConsole.WriteLine();
            
            // Show the current value
            AnsiConsole.MarkupLine($"[yellow]Current {fieldName}:[/] {(string.IsNullOrEmpty(currentValue) ? "[dim]<empty>[/]" : currentValue)}");
            AnsiConsole.WriteLine();
            
            // Show cancel instructions
            AnsiConsole.MarkupLine("[grey]Type [yellow]!cancel[/] to return to main menu[/]");
            AnsiConsole.WriteLine();
            
            // Prompt for the new value
            string prompt = isRequired
                ? $"[green]{fieldName}[/] (required): "
                : $"[grey]{fieldName}[/] (optional): ";
                
            var newValue = isRequired
                ? AnsiConsole.Prompt(
                    new TextPrompt<string>(prompt)
                        .PromptStyle(isRequired ? "green" : "grey")
                        .ValidationErrorMessage($"[red]{fieldName} is required[/]")
                        .Validate(value => 
                            value.Trim() == "!cancel"
                                ? ValidationResult.Success()
                                : !string.IsNullOrWhiteSpace(value) 
                                    ? ValidationResult.Success() 
                                    : ValidationResult.Error($"[red]{fieldName} cannot be empty[/]")))
                : AnsiConsole.Prompt(
                    new TextPrompt<string>(prompt)
                        .AllowEmpty());
                        
            return newValue;
        }

        /// <summary>
        /// Display validation results in a styled format
        /// </summary>
        /// <param name="result">The validation result from Azure Maps</param>
        /// <param name="originalInput">The original address input from the user</param>
        public static void DisplayValidationResult(AddressValidationResult result, AddressInput? originalInput = null)
        {
            AnsiConsole.Clear();
            
            // Create a header
            var title = result.IsValid 
                ? "[green]Address Validated Successfully[/]" 
                : "[red]Address Validation Failed[/]";
                
            AnsiConsole.Write(new Rule(title).Centered());

            // No figlet title - keeping the display clean and simple
            
            // Create a panel for the main result
            var statusColor = result.IsValid ? "green" : "red";
            var statusText = result.IsValid ? "VALID" : "INVALID";
            
            var markup = new Markup($"[{statusColor}]Status:[/] [{statusColor}]{statusText}[/]\n[blue]Confidence:[/] {result.ConfidencePercentage}%\n\n[blue]Message:[/] {result.ValidationMessage}");
            
            var panel = new Panel(markup)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 1, 2, 1),
                BorderStyle = new Style(foreground: result.IsValid ? Color.Green : Color.Red),
                Header = new PanelHeader("Validation Result"),
                Expand = true
            };
            
            // Center the panel in the console
            AnsiConsole.Write(Align.Center(panel));
            
            // Display the original user input if available
            if (originalInput != null)
            {
                var inputParts = new List<string>
                {
                    $"[blue]Address Line 1:[/] {originalInput.AddressLine1}"
                };
                
                if (!string.IsNullOrWhiteSpace(originalInput.AddressLine2))
                    inputParts.Add($"[blue]Address Line 2:[/] {originalInput.AddressLine2}");
                
                if (!string.IsNullOrWhiteSpace(originalInput.AddressLine3))
                    inputParts.Add($"[blue]Address Line 3:[/] {originalInput.AddressLine3}");
                
                inputParts.Add($"[blue]Postal Code:[/] {originalInput.PostalCode}");
                inputParts.Add($"[blue]City:[/] {originalInput.City}");
                inputParts.Add($"[blue]Country:[/] {originalInput.Country}");
                
                var userInputContent = string.Join("\n", inputParts);
                var userInputMarkup = new Markup(userInputContent);
                
                var userInputPanel = new Panel(userInputMarkup)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(2, 1, 2, 1),
                    BorderStyle = new Style(foreground: Color.Yellow),
                    Header = new PanelHeader("Your Input"),
                    Expand = true
                };
                
                AnsiConsole.WriteLine();
                AnsiConsole.Write(Align.Center(userInputPanel));
            }
            
            // Display the formatted address if available
            if (!string.IsNullOrEmpty(result.FreeformAddress))
            {
                // Create markup for address with position data if available
                string addressContent = result.FreeformAddress;
                
                // Add position data as small text below address if available
                if (result.Position != null)
                {
                    addressContent += $"\n\n[grey]Location: {result.Position.Lat:F4}, {result.Position.Lon:F4}[/]";
                }
                
                var addressMarkup = new Markup(addressContent);
                var addressPanel = new Panel(addressMarkup)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(2, 1, 2, 1),
                    BorderStyle = new Style(foreground: Color.Blue),
                    Header = new PanelHeader("Formatted Address"),
                    Expand = true
                };
                
                AnsiConsole.WriteLine();
                AnsiConsole.Write(Align.Center(addressPanel));
            }
        }
    }
}