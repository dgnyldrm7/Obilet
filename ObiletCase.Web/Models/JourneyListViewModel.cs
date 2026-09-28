using ObiletCase.Core.DTOs.BusJourney;

namespace ObiletCase.Web.Models
{
    public class JourneyListViewModel
    {
        public int OriginId { get; set; }
        public string OriginName { get; set; } = string.Empty;

        public int DestinationId { get; set; }
        public string DestinationName { get; set; } = string.Empty;

        public DateTime DepartureDate { get; set; }
        public List<JourneyDto> Journeys { get; set; } = new();

        public JourneyListViewModel() { }

        public JourneyListViewModel(SearchViewModel search, List<JourneyDto>? journeys)
        {
            OriginId = search.OriginId;
            OriginName = search.OriginName ?? "Kalkış";
            DestinationId = search.DestinationId;
            DestinationName = search.DestinationName ?? "Varış";
            DepartureDate = search.DepartureDate;
            Journeys = journeys ?? new();
        }
    }
}