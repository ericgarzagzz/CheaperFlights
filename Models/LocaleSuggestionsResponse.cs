using Newtonsoft.Json;

namespace CheaperFlights.Models
{
    internal class LocaleSuggestionsResponse
    {
        public IEnumerable<LocaleSuggestionGroup> Items { get; set; } = Enumerable.Empty<LocaleSuggestionGroup>();
    }

    internal class LocaleSuggestionGroup
    {
        public string Group { get; set; }
        public string Display { get; set; }
        public IEnumerable<LocaleSuggestion> Items { get; set; } = Enumerable.Empty<LocaleSuggestion>();
    }

    internal class LocaleSuggestion
    {
        public string Id { get; set; }
        public LocaleSuggestionTarget Target { get; set; }
        public string Display { get; set; }
        public string Highlight { get; set; }
        public Location Location { get; set; }

        public override string ToString() => $"{Display} - {Target.Type}";
    }

    internal class LocaleSuggestionTarget
    {
        public string Id { get; set; }
        public string GId { get; set; }
        public string? Iata { get; set; }
        public string? Code { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public object? Parents { get; set; }
    }

    internal class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
