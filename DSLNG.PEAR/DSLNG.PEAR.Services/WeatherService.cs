

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.Weather;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class WeatherService : BaseService, IWeatherService
    {
        public WeatherService(IDataContext dataContext) : base(dataContext) { }

        public GetWeatherResponse GetWeather(GetWeatherRequest request)
        {
            if (request.Date.HasValue)
            {
                var weather = DataContext.Weathers.Include(x => x.Value).FirstOrDefault(x => x.Date == request.Date.Value);
                if (weather != null)
                {
                    var resp = weather.MapTo<GetWeatherResponse>();
                    resp.ValueId = weather.Value.Id;
                    return resp;
                }
                return new GetWeatherResponse();
            }
            else
            {
                var weather = DataContext.Weathers.FirstOrDefault(x => x.Id == request.Id);
                var resp = weather.MapTo<GetWeatherResponse>();
                resp.ValueId = weather.Value.Id;
                return resp;
            }
        }

        public GetWeathersResponse GetWeathers(GetWeathersRequest request)
        {
            var query = DataContext.Weathers.AsQueryable();
            if (request.OnlyCount)
            {
                return new GetWeathersResponse { Count = query.Count() };
            }
            else {
                query = query.Include(x => x.Value);
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetWeathersResponse
                {
                    Weathers = query.MapTo<GetWeathersResponse.WeatherResponse>()
                };
            }
        }

        public SaveWeatherResponse SaveWeather(SaveWeatherRequest request)
        {
            try
            {
                if (request.Id != 0)
                {
                    var weather = DataContext.Weathers.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<Weather>(weather);
                    var value = new SelectOption { Id = request.ValueId };
                    DataContext.SelectOptions.Attach(value);
                    weather.Value = value;
                }
                else
                {
                    var weather = request.MapTo<Weather>();
                    var value = new SelectOption { Id = request.ValueId };
                    DataContext.SelectOptions.Attach(value);
                    weather.Value = value;
                    DataContext.Weathers.Add(weather);
                }
                DataContext.SaveChanges();
                return new SaveWeatherResponse
                {
                    IsSuccess = true,
                    Message = "Weather data has been saved successfully"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveWeatherResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to save weather data"
                };
            }
        }

        public DeleteWeatherResponse Delete(DeleteWeatherRequest request)
        {
            try
            {
                var weather = new Weather { Id = request.Id };
                DataContext.Weathers.Attach(weather);
                DataContext.Weathers.Remove(weather);
                DataContext.SaveChanges();
                return new DeleteWeatherResponse
                {
                    IsSuccess = true,
                    Message = "The highlight has been deleted successfully"
                };
            }
            catch (InvalidOperationException)
            {
                return new DeleteWeatherResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this highlight"
                };
            }
        }
    }
}
