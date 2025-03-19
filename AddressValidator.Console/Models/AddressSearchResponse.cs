namespace AddressValidator.Console.Models
{
    public class AddressSearchResponse
    {
        public Summary Summary { get; set; } = new Summary();
        public AddressSearchResult[] Results { get; set; } = Array.Empty<AddressSearchResult>();
    }

    public class Summary
    {
        // These fields are always present in a valid response according to the docs
        public string Query { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
        public int QueryTime { get; set; }
        public int NumResults { get; set; }
        public int Offset { get; set; }
        public int TotalResults { get; set; }
        public int FuzzyLevel { get; set; }
    }
}
