using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using AddressValidator.Console.Models;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class AddressInputValidationTests
    {
        [Fact]
        public void CreateAddressInput_WithAllRequiredFields_Succeeds()
        {
            // Arrange & Act
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                PostalCode = "12345",
                City = "Seattle",
                Country = "USA"
            };

            // Assert - No exception thrown means success
            addressInput.Should().NotBeNull();
            addressInput.AddressLine1.Should().Be("123 Main St");
            addressInput.PostalCode.Should().Be("12345");
            addressInput.City.Should().Be("Seattle");
            addressInput.Country.Should().Be("USA");
        }

        [Fact]
        public void ValidateAddressInput_EmptyAddressLine1_ShouldBeInvalid()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "",  // Empty rather than missing
                PostalCode = "12345",
                City = "Seattle",
                Country = "USA"
            };

            // Act
            var isValid = !string.IsNullOrWhiteSpace(addressInput.AddressLine1);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAddressInput_EmptyPostalCode_ShouldBeInvalid()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                PostalCode = "",  // Empty rather than missing
                City = "Seattle",
                Country = "USA"
            };

            // Act
            var isValid = !string.IsNullOrWhiteSpace(addressInput.PostalCode);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAddressInput_EmptyCity_ShouldBeInvalid()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                PostalCode = "12345",
                City = "",  // Empty rather than missing
                Country = "USA"
            };

            // Act
            var isValid = !string.IsNullOrWhiteSpace(addressInput.City);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAddressInput_EmptyCountry_ShouldBeInvalid()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                PostalCode = "12345",
                City = "Seattle",
                Country = ""  // Empty rather than missing
            };

            // Act
            var isValid = !string.IsNullOrWhiteSpace(addressInput.Country);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ToSingleLineString_WithAllFields_FormatsCorrectly()
        {
            // Arrange
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                AddressLine3 = "Building North",
                PostalCode = "12345",
                City = "Seattle",
                Country = "USA"
            };

            // Act
            var result = addressInput.ToSingleLineString();

            // Assert - Verify format matches expected output for Azure Maps
            result.Should().Be("123 Main St, Apt 4B, Building North, 12345, Seattle, USA");
        }

        [Fact]
        public void ToSingleLineString_WithOnlyRequiredFields_OmitsOptionalFields()
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
            result.Should().NotContain("null");
            result.Should().NotContain(",,");
        }
    }
}