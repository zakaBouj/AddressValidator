using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Repositories;
using FluentAssertions;
using Xunit;

namespace AddressValidator.Test.Repositories
{
    public class JsonAddressValidationRepositoryTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly JsonAdressValidationRepository _repository;
        private readonly int _maxHistorySize = 5;

        public JsonAddressValidationRepositoryTests()
        {
            // Create a unique test file path in the temp directory
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_validation_history_{Guid.NewGuid()}.json");
            _repository = new JsonAdressValidationRepository(_testFilePath, _maxHistorySize);
        }

        public void Dispose()
        {
            // Clean up test file after tests
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public async Task SaveAndLoadValidationHistory_ShouldWorkCorrectly()
        {
            // Arrange
            var query = "123 Test Street, Test City";
            var addressInput = new AddressInput
            {
                AddressLine1 = "123 Test Street",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            };
            
            var validationResult = new AddressValidationResult
            {
                IsValid = true,
                ConfidencePercentage = 95.5,
                FreeformAddress = "123 Test Street, Test City, 12345"
            };

            // Act - Save a validation result
            await _repository.SaveValidationResultAsync(query, addressInput, validationResult);

            // Assert - Verify it was saved and can be retrieved
            var history = await _repository.GetValidationHistoryAsync();
            var records = history.ToList();
            
            records.Should().HaveCount(1);
            var record = records.First();
            record.OriginalQuery.Should().Be(query);
            record.OriginalAddressInput.Should().BeEquivalentTo(addressInput);
            record.ValidationResult.Should().BeEquivalentTo(validationResult);
        }

        [Fact]
        public async Task SaveMultipleResults_ShouldReturnMostRecentFirst()
        {
            // Arrange
            var queries = new[] { "First query", "Second query", "Third query" };

            // Act - Save multiple results
            for (int i = 0; i < queries.Length; i++)
            {
                await _repository.SaveValidationResultAsync(
                    queries[i], 
                    null, 
                    new AddressValidationResult { 
                        IsValid = true, 
                        ConfidencePercentage = 80 + i
                    });
                
                // Add a small delay to ensure Timestamp differs
                await Task.Delay(50);
            }

            // Assert - The most recent should be first
            var history = await _repository.GetValidationHistoryAsync();
            var records = history.ToList();
            
            records.Should().HaveCount(3);
            records[0].OriginalQuery.Should().Be("Third query");
            records[1].OriginalQuery.Should().Be("Second query");
            records[2].OriginalQuery.Should().Be("First query");
        }

        [Fact]
        public async Task ExceedMaxHistorySize_ShouldTrimOldestRecords()
        {
            // Arrange - Add more records than the max history size
            for (int i = 0; i < _maxHistorySize + 3; i++)
            {
                await _repository.SaveValidationResultAsync(
                    $"Query {i}", 
                    null, 
                    new AddressValidationResult { 
                        IsValid = true, 
                        ConfidencePercentage = 90
                    });
                
                // Add a small delay to ensure different timestamps
                await Task.Delay(50);
            }

            // Act
            var history = await _repository.GetValidationHistoryAsync();
            
            // Assert - Only _maxHistorySize records should remain
            history.Should().HaveCount(_maxHistorySize);
            
            // And they should be the most recent ones
            for (int i = 0; i < _maxHistorySize; i++)
            {
                history.ElementAt(i).OriginalQuery.Should()
                    .Be($"Query {_maxHistorySize + 2 - i}");
            }
        }

        [Fact]
        public async Task GetValidationById_ShouldReturnCorrectRecord()
        {
            // Arrange
            var expectedQuery = "Query to find by ID";
            
            // Save a few records
            for (int i = 0; i < 3; i++)
            {
                await _repository.SaveValidationResultAsync(
                    $"Query {i}", 
                    null, 
                    new AddressValidationResult { IsValid = true });
            }
            
            // Save one record and capture its ID
            await _repository.SaveValidationResultAsync(
                expectedQuery, 
                null, 
                new AddressValidationResult { IsValid = true });
            
            var history = await _repository.GetValidationHistoryAsync();
            var targetId = history.First(r => r.OriginalQuery == expectedQuery).Id;

            // Act
            var result = await _repository.GetValidationByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.OriginalQuery.Should().Be(expectedQuery);
        }
        
        [Fact]
        public async Task GetValidationById_WithInvalidId_ShouldReturnNull()
        {
            // Arrange - Save a record
            await _repository.SaveValidationResultAsync(
                "Test query", 
                null, 
                new AddressValidationResult { IsValid = true });
            
            // Act - Attempt to get a record with an invalid ID
            var result = await _repository.GetValidationByIdAsync(Guid.NewGuid().ToString());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ClearHistory_ShouldRemoveAllRecords()
        {
            // Arrange - Save some records
            for (int i = 0; i < 3; i++)
            {
                await _repository.SaveValidationResultAsync(
                    $"Query {i}", 
                    null, 
                    new AddressValidationResult { IsValid = true });
            }
            
            // Verify we have records
            var initialHistory = await _repository.GetValidationHistoryAsync();
            initialHistory.Should().HaveCount(3);

            // Act
            var result = await _repository.ClearHistoryAsync();
            var finalHistory = await _repository.GetValidationHistoryAsync();

            // Assert
            result.Should().BeTrue(); // Operation should succeed
            finalHistory.Should().BeEmpty(); // History should be empty
        }
        
        [Fact]
        public async Task ClearHistory_WhenNoFile_ShouldReturnFalse()
        {
            // Arrange - Make sure file doesn't exist
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            
            // Create a new repository instance to avoid cached data
            var freshRepository = new JsonAdressValidationRepository(_testFilePath);
            
            // Act
            var result = await freshRepository.ClearHistoryAsync();
            
            // Assert
            result.Should().BeFalse(); // Should return false since there was no file
        }
    }
}