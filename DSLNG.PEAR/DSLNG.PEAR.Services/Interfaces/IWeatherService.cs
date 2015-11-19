
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.Weather;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IWeatherService
    {
        GetWeatherResponse GetWeather(GetWeatherRequest request);
        GetWeathersResponse GetWeathers(GetWeathersRequest request);
        SaveWeatherResponse SaveWeather(SaveWeatherRequest request);
        DeleteWeatherResponse Delete(DeleteWeatherRequest request);
        GetWeathersResponse GetWeathersForGrid(GetWeathersRequest request);
    }
}
