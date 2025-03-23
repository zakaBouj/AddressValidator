namespace AddressValidator.Console.Models
{
    public class AddressValidationResult
    {
        public bool IsValid { get; set; }

        public double ConfidencePercentage { get; set; }
        public string? FreeformAddress { get; set; }
        public string? ValidationMessage { get; set; }

        public AddressInput? OriginalInput { get; set; }
        public Address? MatchedAddress { get; set; }

        public Position? Position { get; set; }
    }
}