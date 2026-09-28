namespace ObiletCase.Web.Models
{
    public class SearchViewModel
    {
        public int OriginId { get; set; }
        public string? OriginName { get; set; }

        public int DestinationId { get; set; }
        public string? DestinationName { get; set; }

        public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(1);
    }
}
