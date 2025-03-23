namespace AddressValidator.Console.Models
{
    public class AddressSearchResult
    {
        // Core fields that are always present in a successful result
        public string Type { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public double Score { get; set; }
        
        // Position is always returned for a geographic result
        public Position Position { get; set; } = new Position();
        
        // These may be optional depending on result type
        public Address? Address { get; set; }
        public Viewport? Viewport { get; set; }
        public EntryPoint[]? EntryPoints { get; set; }
    }

    public class Address
    {
        // Address components vary by region and result type
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }
        public string? MunicipalitySubdivision { get; set; }
        public string? Municipality { get; set; }
        public string? CountrySecondarySubdivision { get; set; }
        public string? CountryTertiarySubdivision { get; set; }
        public string? CountrySubdivisionCode { get; set; }
        public string? PostalCode { get; set; }
        public string? ExtendedPostalCode { get; set; }
        
        // These fields are typically always present for an address
        public string? CountryCode { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? CountryCodeISO3 { get; set; }
        public string? FreeformAddress { get; set; } = string.Empty;
        public string? CountrySubdivisionName { get; set; }
    }

    public class Position
    {
        // Coordinate values are always present
        public double Lat { get; set; }
        public double Lon { get; set; } 
    }

    public class Viewport
    {
        // Viewport corners are always present when viewport is included
        public TopLeftPoint TopLeftPoint { get; set; } = new TopLeftPoint();
        public BtmRightPoint BtmRightPoint { get; set; } = new BtmRightPoint();
    }

    public class EntryPoint
    {
        public string Type { get; set; } = string.Empty;
        public Position Position { get; set; } = new Position();
    }

    public class TopLeftPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class BtmRightPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
