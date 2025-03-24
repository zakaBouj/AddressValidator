using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Repositories;
using FluentAssertions;
using Xunit;
using SystemConsole = System.Console; // Add alias to avoid namespace conflict

namespace AddressValidator.Test.Integration
{
    public class AddressValidationRepositoryIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _integrationFilePath;
        private readonly int _maxHistorySize = 100;

        public AddressValidationRepositoryIntegrationTests()
        {
            // Create a test directory in the application's output directory
            _testDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "IntegrationTests",
                Guid.NewGuid().ToString());
            
            Directory.CreateDirectory(_testDirectory);
            
            _integrationFilePath = Path.Combine(_testDirectory, "integration_history.json");
        }

        public void Dispose()
        {
            // Clean up test directory after tests
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (IOException)
                {
                    // If we can't delete immediately, don't fail the test
                    // This can happen with file system locking
                    SystemConsole.WriteLine($"Warning: Could not delete test directory {_testDirectory}");
                }
            }
        }

        [Fact]
        public async Task DataPersistsBetweenRepositoryInstances()
        {
            // Arrange
            var firstRepository = new JsonAdressValidationRepository(_integrationFilePath);
            var testQuery = "Integration Test Address";
            var testResult = new AddressValidationResult { IsValid = true, ConfidencePercentage = 99.9 };
            
            // Act - Save data with first repository instance
            await firstRepository.SaveValidationResultAsync(testQuery, null, testResult);
            
            // Create a completely new repository instance pointing to the same file
            var secondRepository = new JsonAdressValidationRepository(_integrationFilePath);
            var loadedHistory = await secondRepository.GetValidationHistoryAsync();
            
            // Assert
            loadedHistory.Should().HaveCount(1);
            var record = loadedHistory.First();
            record.OriginalQuery.Should().Be(testQuery);
            record.ValidationResult.ConfidencePercentage.Should().Be(99.9);
            record.ValidationResult.IsValid.Should().BeTrue();
            
            // Verify the file exists on disk
            File.Exists(_integrationFilePath).Should().BeTrue();
        }

        [Fact]
        public async Task LargeDatasetPerformanceTest()
        {
            // Arrange
            var repository = new JsonAdressValidationRepository(_integrationFilePath, _maxHistorySize);
            var stopwatch = new Stopwatch();
            const int batchSize = 500; // Large dataset size
            
            // Act - Time how long it takes to save many records
            stopwatch.Start();
            
            for (int i = 0; i < batchSize; i++)
            {
                await repository.SaveValidationResultAsync(
                    $"Performance Test Address {i}",
                    new AddressInput
                    {
                        AddressLine1 = $"Address Line {i}",
                        City = "Test City",
                        PostalCode = $"{10000 + i}",
                        Country = "Test Country"
                    },
                    new AddressValidationResult
                    {
                        IsValid = i % 2 == 0, // Alternate between valid and invalid
                        ConfidencePercentage = 50 + (i % 50), // Vary confidence score
                        FreeformAddress = $"Address Line {i}, Test City, {10000 + i}"
                    }
                );
            }
            
            var saveTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            
            // Load the history and measure time
            var history = await repository.GetValidationHistoryAsync(_maxHistorySize);
            var records = history.ToList();
            
            var loadTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            
            // Assert
            // 1. We should only have _maxHistorySize records due to trimming (or less if batchSize < _maxHistorySize)
            records.Should().HaveCount(Math.Min(batchSize, _maxHistorySize));
            
            // 2. Performance checks - these thresholds are somewhat arbitrary and may need adjustment
            // but provide a baseline for detecting major performance regressions
            saveTime.Should().BeLessThan(5000, "Saving should complete in a reasonable time");
            loadTime.Should().BeLessThan(1000, "Loading should be quick");
            
            // 3. The first record should be the last one we added
            records.First().OriginalQuery.Should().Be($"Performance Test Address {batchSize - 1}");
            
            // 4. File size should be reasonable
            var fileInfo = new FileInfo(_integrationFilePath);
            fileInfo.Exists.Should().BeTrue();
            
            // Log performance metrics
            SystemConsole.WriteLine($"Save time for {batchSize} records: {saveTime}ms");
            SystemConsole.WriteLine($"Load time for {Math.Min(batchSize, _maxHistorySize)} records: {loadTime}ms");
            SystemConsole.WriteLine($"File size: {fileInfo.Length / 1024} KB");
        }
        
        [Fact]
        public async Task MultipleRepositoryInstancesTest()
        {
            // Arrange
            const int operationCount = 20;
            var repositoryInstances = new List<JsonAdressValidationRepository>();
            
            // Create multiple repository instances - simulates multiple users/processes
            for (int i = 0; i < 5; i++)
            {
                repositoryInstances.Add(new JsonAdressValidationRepository(_integrationFilePath));
            }
            
            // Act - Save records using different repository instances (sequentially)
            for (int i = 0; i < operationCount; i++)
            {
                // Use different repository instances in round-robin fashion
                var repoIndex = i % repositoryInstances.Count;
                await repositoryInstances[repoIndex].SaveValidationResultAsync(
                    $"Multi-Instance Test {i}",
                    null,
                    new AddressValidationResult { IsValid = i % 2 == 0 }
                );
            }
            
            // Create a fresh repository instance to query the results
            var queryRepository = new JsonAdressValidationRepository(_integrationFilePath);
            var history = await queryRepository.GetValidationHistoryAsync(operationCount);
            var records = history.ToList();
            
            // Assert
            // We should have the correct number of records
            records.Should().HaveCount(operationCount);
            
            // Check that all our operations are recorded (order may vary)
            var queries = records.Select(r => r.OriginalQuery).OrderBy(q => q).ToList();
            var expectedQueries = Enumerable.Range(0, operationCount)
                .Select(i => $"Multi-Instance Test {i}")
                .OrderBy(q => q)
                .ToList();
            
            queries.Should().BeEquivalentTo(expectedQueries);
            
            // Verify that the isValid property alternates as expected
            foreach (var record in records)
            {
                int index = int.Parse(record.OriginalQuery.Split(' ').Last());
                record.ValidationResult.IsValid.Should().Be(index % 2 == 0);
            }
        }
    }
}