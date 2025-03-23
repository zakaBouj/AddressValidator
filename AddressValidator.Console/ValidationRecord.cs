namespace AddressValidator.Console.Models
{
    public class ValidationRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string OriginalQuery { get; set; } = string.Empty;
        public AddressInput? OriginalAddressInput { get; set; }
        public AddressValidationResult Result { get; set; } = null!;

        public string Summary => 
        $"[{(Result.IsValid ? "VALID" : "INVALID")}] {Result.FreeformAddress ?? OriginalQuery} ({Result.ConfidencePercentage}%)";
    }
}