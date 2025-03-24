using System;
using AddressValidator.Console.Models;
using AddressValidator.Console.UI;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace AddressValidator.Test.UI
{
    public class AddressFormUITests
    {
        [Fact]
        public void FormatValueForDisplay_WithEmptyValue_ReturnsEmptyIndicator()
        {
            // Arrange
            string emptyValue = string.Empty;
            string nullValue = null;
            string whitespaceValue = "   ";
            
            // Act 
            // Use reflection to call the private method
            var result1 = InvokeFormatValueForDisplay(emptyValue);
            var result2 = InvokeFormatValueForDisplay(nullValue);
            var result3 = InvokeFormatValueForDisplay(whitespaceValue);
            
            // Assert
            result1.Should().Be("[dim]<empty>[/]");
            result2.Should().Be("[dim]<empty>[/]");
            result3.Should().Be("[dim]<empty>[/]");
        }
        
        [Fact]
        public void FormatValueForDisplay_WithNonEmptyValue_ReturnsBoldFormattedValue()
        {
            // Arrange
            string testValue = "Test Value";
            
            // Act
            var result = InvokeFormatValueForDisplay(testValue);
            
            // Assert
            result.Should().Be($"[bold]{testValue}[/]");
        }
        
        [Fact]
        public void AddressInput_CanConvertToSingleLineString()
        {
            // Arrange
            var input = new AddressInput
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                AddressLine3 = "Building C",
                PostalCode = "12345",
                City = "Test City",
                Country = "Test Country"
            };
            
            // Act
            var singleLine = input.ToSingleLineString();
            
            // Assert
            singleLine.Should().Contain("123 Main St");
            singleLine.Should().Contain("Apt 4B");
            singleLine.Should().Contain("Building C");
            singleLine.Should().Contain("12345");
            singleLine.Should().Contain("Test City");
            singleLine.Should().Contain("Test Country");
            singleLine.Should().Contain(", "); // Elements should be comma-separated
        }
        
        [Fact]
        public void AddressValidationResult_CanUseConfidencePercentage()
        {
            // Arrange
            var result = new AddressValidationResult
            {
                ConfidencePercentage = 85.64,
                IsValid = true
            };
            
            // Act & Assert
            result.ConfidencePercentage.Should().BeGreaterThan(85);
            result.ConfidencePercentage.Should().BeLessThan(86);
        }
        
        [Fact]
        public void AddressValidationResult_CanBeCreatedWithAllProperties()
        {
            // Arrange & Act
            var result = new AddressValidationResult
            {
                IsValid = true,
                ConfidencePercentage = 95.0,
                ValidationMessage = "Address is valid",
                FreeformAddress = "123 Test St, Test City",
                Position = new Position { Lat = 42.0, Lon = -71.0 }
            };
            
            // Assert
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().Be(95.0);
            result.ValidationMessage.Should().Be("Address is valid");
            result.FreeformAddress.Should().Be("123 Test St, Test City");
            result.Position.Should().NotBeNull();
            result.Position.Lat.Should().Be(42.0);
            result.Position.Lon.Should().Be(-71.0);
        }
        
        // Helper to invoke the private FormatValueForDisplay method via reflection
        private static string InvokeFormatValueForDisplay(string value)
        {
            var methodInfo = typeof(AddressFormUI).GetMethod(
                "FormatValueForDisplay", 
                BindingFlags.NonPublic | BindingFlags.Static
            );
            
            return (string)methodInfo.Invoke(null, new object[] { value });
        }
    }
}