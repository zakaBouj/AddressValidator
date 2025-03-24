using System;
using System.IO;
using System.Text;
using Xunit;

namespace AddressValidator.Test.UI
{
    /// <summary>
    /// Helper class for testing console UI components
    /// </summary>
    public class UITestHelpers
    {
        /// <summary>
        /// Captures console output from a method that writes to System.Console
        /// </summary>
        public static string CaptureConsoleOutput(Action action)
        {
            // Save the original output stream
            var originalOutput = System.Console.Out;
            
            try
            {
                // Create a new StringWriter to capture the output
                using var stringWriter = new StringWriter();
                System.Console.SetOut(stringWriter);
                
                // Execute the action
                action();
                
                // Return the captured output
                return stringWriter.ToString();
            }
            finally
            {
                // Restore the original output stream
                System.Console.SetOut(originalOutput);
            }
        }
        
        /// <summary>
        /// Simulates input for methods that read from System.Console
        /// </summary>
        public static string CaptureConsoleOutputWithInput(Action action, params string[] inputs)
        {
            // Save original input and output streams
            var originalInput = System.Console.In;
            var originalOutput = System.Console.Out;
            
            try
            {
                // Create a StringReader with the provided inputs
                var inputString = string.Join(Environment.NewLine, inputs);
                var stringReader = new StringReader(inputString);
                System.Console.SetIn(stringReader);
                
                // Create a StringWriter to capture the output
                using var stringWriter = new StringWriter();
                System.Console.SetOut(stringWriter);
                
                // Execute the action
                action();
                
                // Return the captured output
                return stringWriter.ToString();
            }
            finally
            {
                // Restore original input and output streams
                System.Console.SetIn(originalInput);
                System.Console.SetOut(originalOutput);
            }
        }
    }
}