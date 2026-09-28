using ObiletCase.Core.DTOs.BusLocation;

namespace ObiletCase.Web.Models
{
    public class HomeIndexViewModel
    {
        private const int DefaultIstanbulAvrupaId = 349;
        private const int DefaultAnkaraId = 356;

        public IReadOnlyList<BusLocationDto> Locations { get; }
        public int DefaultOriginId { get; }
        public int DefaultDestinationId { get; }

        public HomeIndexViewModel(List<BusLocationDto>? locations = null)
        {
            Locations = locations ?? new List<BusLocationDto>();

            var defaultOrigin = Locations.FirstOrDefault(x => x.Id == DefaultIstanbulAvrupaId)
                                ?? Locations.FirstOrDefault(x => x.Name.Contains("İstanbul Avrupa", StringComparison.OrdinalIgnoreCase))
                                ?? Locations.FirstOrDefault();

            var defaultDestination = Locations.FirstOrDefault(x => x.Id == DefaultAnkaraId)
                                     ?? Locations.FirstOrDefault(x => x.Name.Contains("Ankara", StringComparison.OrdinalIgnoreCase))
                                     ?? Locations.FirstOrDefault(x => x.Id != defaultOrigin?.Id);

            DefaultOriginId = defaultOrigin?.Id ?? DefaultIstanbulAvrupaId;
            DefaultDestinationId = defaultDestination?.Id ?? DefaultAnkaraId;
        }
    }
}
