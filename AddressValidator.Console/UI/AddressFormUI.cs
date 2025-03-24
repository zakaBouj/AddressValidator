using Spectre.Console;
using AddressValidator.Console.Models;

namespace AddressValidator.Console.UI
{
    public class AddressFormUI
    {
        /// <summary>
        /// Collect address details from the user with field validation
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
            
            try
            {
                // First collect all the values with cancellation support
                
                // Address Line 1 (required)
                var addressLine1 = AnsiConsole.Prompt(
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
                if (addressLine1.Trim() == "!cancel")
                    return null;
                
                // Address Line 2 (optional)
                var addressLine2 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[grey]Address Line 2[/] (optional): ")
                        .AllowEmpty());
                        
                // Check for cancellation
                if (addressLine2.Trim() == "!cancel")
                    return null;
                
                // Address Line 3 (optional)
                var addressLine3 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[grey]Address Line 3[/] (optional): ")
                        .AllowEmpty());
                        
                // Check for cancellation
                if (addressLine3.Trim() == "!cancel")
                    return null;
                
                // Postal Code (required)
                var postalCode = AnsiConsole.Prompt(
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
                if (postalCode.Trim() == "!cancel")
                    return null;
                
                // City (required)
                var cityValue = AnsiConsole.Prompt(
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
                if (cityValue.Trim() == "!cancel")
                    return null;
                
                // Country (required)
                var countryValue = AnsiConsole.Prompt(
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
                if (countryValue.Trim() == "!cancel")
                    return null;
                
                // Now create the AddressInput object with all collected values
                var addressInput = new AddressInput {
                    AddressLine1 = addressLine1,
                    AddressLine2 = addressLine2,
                    AddressLine3 = addressLine3,
                    PostalCode = postalCode,
                    City = cityValue,
                    Country = countryValue
                };

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