namespace Forecast.Models.Weather
{
    public record DayForecast
    {
        public DateTime Date { get; set; }
        public decimal MinTempreture { get; set; }
        public decimal MaxTempreture { get; set; }
        public decimal Humidity { get; set; }
        public decimal WindSpeed { get; set; }

    }
    public class DailyForecast
    {
        public List<DayForecast> DayForecasts { get; } = new();
    }
}
