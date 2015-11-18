

using System;
namespace DSLNG.PEAR.Services.Requests.Weather
{
    public class GetWeatherRequest
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public bool ByDate { get; set; }
    }
}
