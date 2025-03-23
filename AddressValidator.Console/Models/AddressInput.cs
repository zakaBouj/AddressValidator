namespace AddressValidator.Console.Models
{
    public class AddressInput
    {
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }

        public string ToSingleLineString()
        {
            var addressParts = new List<string>
            {
                AddressLine1,
                // Only add non-empty optional fields
                !string.IsNullOrWhiteSpace(AddressLine2) ? AddressLine2 : string.Empty,
                !string.IsNullOrWhiteSpace(AddressLine3) ? AddressLine3 : string.Empty,
                PostalCode,
                City,
                Country
            };

            return string.Join(", ", addressParts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}