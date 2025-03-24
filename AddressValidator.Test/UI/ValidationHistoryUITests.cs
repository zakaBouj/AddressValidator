using System;
using System.Collections.Generic;
using AddressValidator.Console.Models;
using AddressValidator.Console.UI;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace AddressValidator.Test.UI
{
    public class ValidationHistoryUITests
    {
        [Fact]
        public void SelectRecordFromHistory_WithNullHistory_ThrowsArgumentNullException()
        {
            // In the current implementation, passing null to SelectRecordFromHistory
            // throws an ArgumentNullException. Let's test for this behavior.
            
            // Arrange - Create a null history
            List<ValidationRecord> history = null;
            
            // Act & Assert - Expect an ArgumentNullException
            var exception = Record.Exception(() =>
                ValidationHistoryUI.SelectRecordFromHistory(history));
                
            // The exception should be an ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }
        
        [Fact]
        public void ValidationHistoryUI_CanCreateSampleHistory()
        {
            // This tests our test helper method to ensure it creates valid objects
            
            // Act
            var sampleHistory = CreateSampleValidationHistory();
            
            // Assert
            sampleHistory.Should().NotBeNull();
            sampleHistory.Should().HaveCount(2);
            sampleHistory[0].ValidationResult.IsValid.Should().BeTrue();
            sampleHistory[1].ValidationResult.IsValid.Should().BeFalse();
        }
        
        [Fact]
        public void ValidationRecord_HasCorrectProperties()
        {
            // Testing the structure of the ValidationRecord class
            // This indirectly tests our history functionality
            
            // Arrange
            var record = new ValidationRecord
            {
                Id = "test-id",
                Timestamp = DateTime.Now,
                OriginalQuery = "Test query",
                OriginalAddressInput = new AddressInput
                {
                    AddressLine1 = "Test address",
                    PostalCode = "12345",
                    City = "Test City",
                    Country = "Test Country"
                },
                ValidationResult = new AddressValidationResult 
                {
                    IsValid = true,
                    ConfidencePercentage = 90
                }
            };
            
            // Assert
            record.Id.Should().Be("test-id");
            record.OriginalQuery.Should().Be("Test query");
            record.OriginalAddressInput.Should().NotBeNull();
            record.OriginalAddressInput.AddressLine1.Should().Be("Test address");
            record.ValidationResult.Should().NotBeNull();
            record.ValidationResult.IsValid.Should().BeTrue();
            record.ValidationResult.ConfidencePercentage.Should().Be(90);
        }
        
        // Helper method to create sample validation history
        private List<ValidationRecord> CreateSampleValidationHistory()
        {
            return new List<ValidationRecord>
            {
                new ValidationRecord
                {
                    Id = "1",
                    Timestamp = DateTime.Now.AddDays(-1),
                    OriginalQuery = "123 Test St, Test City, 12345",
                    OriginalAddressInput = new AddressInput
                    {
                        AddressLine1 = "123 Test St",
                        City = "Test City",
                        PostalCode = "12345",
                        Country = "Test Country"
                    },
                    ValidationResult = new AddressValidationResult
                    {
                        IsValid = true,
                        ConfidencePercentage = 95,
                        ValidationMessage = "Address is valid",
                        FreeformAddress = "123 Test St, Test City, 12345",
                        Position = new Position { Lat = 12.34, Lon = 56.78 }
                    }
                },
                new ValidationRecord
                {
                    Id = "2",
                    Timestamp = DateTime.Now.AddDays(-2),
                    OriginalQuery = "Invalid Address, Nowhere, 99999",
                    OriginalAddressInput = new AddressInput
                    {
                        AddressLine1 = "Invalid Address",
                        City = "Nowhere",
                        PostalCode = "99999",
                        Country = "Unknown"
                    },
                    ValidationResult = new AddressValidationResult
                    {
                        IsValid = false,
                        ConfidencePercentage = 10,
                        ValidationMessage = "Address not found",
                        FreeformAddress = null,
                        Position = null
                    }
                }
            };
        }
    }
}