namespace AddressValidator.Console.Models
{
    public class AddressSearchResponse
    {
        public Summary Summary { get; set; }
        public AddressSearchResult[] Results { get; set; }
    }

    public class Summary
    {
        public string Query { get; set; }
        public string QueryType { get; set; }
        public int QueryTime { get; set; }
        public int NumResults { get; set; }
        public int Offset { get; set; }
        public int TotalResults { get; set; }
        public int FuzzyLevel { get; set; }
    }
}