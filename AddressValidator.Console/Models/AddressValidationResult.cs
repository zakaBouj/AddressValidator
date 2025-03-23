namespace AddressValidator.Console.Models
{
    public class AddressValidationResult
    {
        public bool IsValid { get; set; }

        public int ConfidencePercentage { get; set; }
        public string? FormattedAddress { get; set; }
        public string? ValidationMessage { get; set; }

        public AddressInput? OriginalInput { get; set; }
        public Address? MatchedAddress { get; set; }

        public Position? Position { get; set; }
    }
}