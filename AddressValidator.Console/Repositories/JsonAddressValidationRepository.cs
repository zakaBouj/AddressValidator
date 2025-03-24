using System.Text.Json;
using AddressValidator.Console.Models;

namespace AddressValidator.Console.Repositories
{
    public class JsonAdressValidationRepository : IAddressValidationRepository
    {
        private readonly string _jsonFilePath;
        private readonly int _maxHistorySize;

        public JsonAdressValidationRepository(string jsonFilePath, int maxHistroySize = 20)
        {
            _jsonFilePath = jsonFilePath;
            _maxHistorySize = maxHistroySize;

            // Ensure directory exists
            string? directoryName = Path.GetDirectoryName(jsonFilePath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        private async Task<List<ValidationRecord>> LoadHistoryAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return [];

            var json = await File.ReadAllTextAsync(_jsonFilePath);
            if (string.IsNullOrWhiteSpace(json))
                return [];

            return JsonSerializer.Deserialize<List<ValidationRecord>>(json) ?? [];
        }

        private async Task SaveHistoryAsync(List<ValidationRecord> history)
        {
            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true});
            await File.WriteAllTextAsync(_jsonFilePath, json);
        }

        public async Task SaveValidationResultAsync(string originalQuery, AddressInput? addressInput, AddressValidationResult validationResult)
        {
            var history = await LoadHistoryAsync();

            // Creat new record
            var record = new ValidationRecord
            {
                Timestamp = DateTime.UtcNow,
                OriginalQuery = originalQuery,
                OriginalAddressInput = addressInput,
                ValidationResult = validationResult
            };

            // Add to beginning of list (most recent first)
            history.Insert(0, record);

            // Trim history to max size
            if (history.Count > _maxHistorySize)
                history = [.. history.Take(_maxHistorySize)];

            // Save history
            await SaveHistoryAsync(history);
        }

        public async Task<IEnumerable<ValidationRecord>> GetValidationHistoryAsync(int maxResults = 20)
        {
            var history = await LoadHistoryAsync();
            return history.Take(Math.Min(maxResults, history.Count));
        }

        public async Task<ValidationRecord?> GetValidationByIdAsync(string id)
        {
            var history = await LoadHistoryAsync();
            return history.FirstOrDefault(r => r.Id == id);
        }

        public async Task<bool> ClearHistoryAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return false;

            await SaveHistoryAsync([]);
            return true;
        }
    }
}