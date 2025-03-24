using Spectre.Console;
using AddressValidator.Console.Models;

namespace AddressValidator.Console.UI
{
    public class ValidationHistoryUI
    {
        /// <summary>
        /// Display validation history in a table format
        /// </summary>
        public static void DisplayValidationHistory(IEnumerable<ValidationRecord> historyRecords)
        {
            AnsiConsole.Clear();
            
            // Simple title without figlet text
            AnsiConsole.Write(new Rule("[blue]Validation History[/]").Centered());
            
            var records = historyRecords.ToList();
            
            if (records.Count == 0)
            {
                var emptyPanel = new Panel(
                    new Markup("[yellow]No validation history found.[/]")
                )
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(foreground: Color.Yellow),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(Align.Center(emptyPanel));
                return;
            }
            
            // Show record count
            AnsiConsole.Write(
                Align.Center(
                    new Markup($"[blue]Found [green]{records.Count}[/] validation records[/]")
                )
            );
            AnsiConsole.WriteLine();
            
            // Create a table for displaying the records
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Blue)
                .AddColumn(new TableColumn("ID").Centered())
                .AddColumn(new TableColumn("Date/Time").Centered())
                .AddColumn(new TableColumn("Address Query").LeftAligned())
                .AddColumn(new TableColumn("Status").Centered())
                .AddColumn(new TableColumn("Confidence").RightAligned());
            
            // Add each record to the table
            foreach (var (record, index) in records.Select((r, i) => (r, i)))
            {
                var status = record.ValidationResult.IsValid 
                    ? $"[green]VALID[/]" 
                    : $"[red]INVALID[/]";
                
                table.AddRow(
                    (index + 1).ToString(),
                    record.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                    record.OriginalQuery,
                    status,
                    $"{record.ValidationResult.ConfidencePercentage:F2}%"
                );
            }
            
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
        
        /// <summary>
        /// Select a record from validation history
        /// </summary>
        public static ValidationRecord? SelectRecordFromHistory(IEnumerable<ValidationRecord> historyRecords)
        {
            // Add explicit null check at the beginning of the method
            if (historyRecords == null)
                throw new ArgumentNullException(nameof(historyRecords));
                
            var records = historyRecords.ToList();
            
            if (records.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No validation history found.[/]");
                return null;
            }
            
            AnsiConsole.Clear();
            
            // Simple title without figlet text
            AnsiConsole.Write(new Rule("[blue]Select Address to Re-validate[/]").Centered());
            
            // Add description
            AnsiConsole.Write(
                Align.Center(
                    new Panel(
                        new Markup("[yellow]Select an address from your validation history to re-check with Azure Maps[/]")
                    )
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(foreground: Color.Blue),
                        Padding = new Padding(2, 1, 2, 1)
                    }
                )
            );
            
            AnsiConsole.WriteLine();
            
            // Order records by time, newest first (already done in repository)
            
            // Remove duplicate address queries, keeping only the most recent
            var uniqueRecords = records
                .GroupBy(r => r.OriginalQuery)
                .Select(g => g.OrderByDescending(r => r.Timestamp).First())
                .ToList();
            
            // Create categorized lists for valid and invalid addresses, ordered by timestamp (newest first)
            var validRecords = uniqueRecords
                .Where(r => r.ValidationResult.IsValid)
                .OrderByDescending(r => r.Timestamp)
                .ToList();
                
            var invalidRecords = uniqueRecords
                .Where(r => !r.ValidationResult.IsValid)
                .OrderByDescending(r => r.Timestamp)
                .ToList();
            
            // Create the selection prompt with styled grouping
            var selectionPrompt = new SelectionPrompt<string>()
                .Title("[blue]Select an address from history:[/]")
                .PageSize(15)
                .HighlightStyle(new Style(foreground: Color.Blue, background: Color.Grey))
                .AddChoices(new[] { "[grey]Cancel[/]" });
            
            // Add valid addresses if any exist
            if (validRecords.Any())
            {
                selectionPrompt.AddChoiceGroup("[green]Valid Addresses[/]",
                    validRecords.Select((r, i) =>
                        $"V{i + 1}. {r.OriginalQuery} [grey]({r.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm})[/]"));
            }
            
            // Add invalid addresses if any exist
            if (invalidRecords.Any())
            {
                selectionPrompt.AddChoiceGroup("[red]Invalid Addresses[/]",
                    invalidRecords.Select((r, i) =>
                        $"I{i + 1}. {r.OriginalQuery} [grey]({r.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm})[/]"));
            }
            
            // Show the selection prompt
            var selection = AnsiConsole.Prompt(selectionPrompt);
            
            // Handle cancel option
            if (selection == "[grey]Cancel[/]")
                return null;
            
            // Extract the identifier from the selection string
            var prefix = selection[0]; // 'V' or 'I'
            var indexEnd = selection.IndexOf('.');
            var indexStr = selection.Substring(1, indexEnd - 1);
            var index = int.Parse(indexStr) - 1;
            
            // Get the record based on the prefix and index
            if (prefix == 'V')
                return validRecords[index];
            else // prefix == 'I'
                return invalidRecords[index];
        }
        
        /// <summary>
        /// Confirm clearing of validation history
        /// </summary>
        public static bool ConfirmClearHistory()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[red]Clear Validation History[/]").Centered());
            
            // Show warning panel
            var panel = new Panel(
                Align.Center(
                    new Markup("[bold red]Warning:[/] This action will permanently delete all validation history records.\nThis cannot be undone.")
                )
            )
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(foreground: Color.Red),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
            
            // Confirm with the user
            return AnsiConsole.Confirm("Are you sure you want to clear all validation history?", false);
        }
        
        /// <summary>
        /// Show success message after clearing history
        /// </summary>
        public static void ShowClearSuccessMessage()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel(
                Align.Center(
                    new Markup("[green]Validation history cleared successfully.[/]")
                )
            )
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(foreground: Color.Green),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(panel);
        }
        
        /// <summary>
        /// Show message when no history exists to clear
        /// </summary>
        public static void ShowNoHistoryToClear()
        {
            AnsiConsole.Clear();
            
            var panel = new Panel(
                Align.Center(
                    new Markup("[yellow]No validation history found to clear.[/]")
                )
            )
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(foreground: Color.Yellow),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(panel);
        }
    }
}