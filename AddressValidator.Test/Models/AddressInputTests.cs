using System;
using System.Collections.Generic;
using Xunit;
using AddressValidator.Console.Models;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class AddressInputTests
    {
        [Fact]
        public void ToSingleLineString_WithAllFields_ReturnsFormattedString()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                AddressLine3 = "Building C",
                PostalCode = "12345",
                City = "Seattle",
                Country = "USA"
            };

            // Act
            var result = addressInput.ToSingleLineString();

            // Assert
            result.Should().Be("123 Main St, Apt 4B, Building C, 12345, Seattle, USA");
        }

        [Fact]
        public void ToSingleLineString_WithOnlyRequiredFields_ReturnsFormattedString()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = null,
                AddressLine3 = null,
                PostalCode = "12345",
                City = "Seattle",
                Country = "USA"
            };

            // Act
            var result = addressInput.ToSingleLineString();

            // Assert
            result.Should().Be("123 Main St, 12345, Seattle, USA");
        }
    }
}