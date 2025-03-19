namespace AddressValidator.Console.Models
{
    public class AddressSearchResult
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public double Score { get; set; }
        public Address Address { get; set; }
        public Position Position { get; set; }
        public Viewport Viewport { get; set; }
        public EntryPoint[] EntryPoints { get; set; }        
    }

    public class Address
    {
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string MunicipalitySubdivision { get; set; }
        public string Municipality { get; set; }
        public string CountrySecondarySubdivision { get; set; }
        public string CountryTertiarySubdivision { get; set; }
        public string CountrySubdivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string ExtendedPostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string CountryCodeISO3 { get; set; }
        public string FreeformAddress { get; set; }
        public string CountrySubdivisionName { get; set; }
    }

    public class Position
    {
        public double Lat { get; set; }
        public double Lon { get; set; } 
    }

    public class Viewport
    {
        public TopLeftPoint TopLeftPoint { get; set; }
        public BtmRightPoint BtmRightPoint { get; set; }
    }

    public class EntryPoint
    {
        public string Type { get; set; }
        public Position Position { get; set; }
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