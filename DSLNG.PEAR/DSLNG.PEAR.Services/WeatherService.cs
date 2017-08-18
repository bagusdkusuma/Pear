

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.Weather;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
                if (request.ByDate)
                {
                    var weather = DataContext.Weathers.Include(x => x.Value).OrderByDescending(x => x.Date).FirstOrDefault();
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
                    var weather = DataContext.Weathers.Include(x => x.Value).FirstOrDefault(x => x.Id == request.Id);
                    var resp = weather.MapTo<GetWeatherResponse>();
                    resp.ValueId = weather.Value.Id;
                    return resp;
                }
            }
        }

        public GetWeathersResponse GetWeathers(GetWeathersRequest request)
        {
            var query = DataContext.Weathers.AsQueryable();
            if (request.OnlyCount)
            {
                return new GetWeathersResponse { Count = query.Count() };
            }
            else
            {
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
                var weather = request.MapTo<Weather>();
                var action = request.MapTo<BaseAction>();
                if (request.Id != 0)
                {
                    weather = DataContext.Weathers.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<Weather>(weather);
                    var value = new SelectOption { Id = request.ValueId };
                    DataContext.SelectOptions.Attach(value);
                    weather.Value = value;
                }
                else
                {

                    var value = new SelectOption { Id = request.ValueId };
                    DataContext.SelectOptions.Attach(value);
                    weather.Value = value;
                    DataContext.Weathers.Add(weather);
                }
                DataContext.SaveChanges(action);
                return new SaveWeatherResponse
                {
                    IsSuccess = true,
                    Message = "Weather data has been saved successfully",
                    Id = weather.Id
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveWeatherResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to save weather data"
                };
            }
            catch (Exception e)
            {
                return new SaveWeatherResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to save weather data, Error message = " + e.Message
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
                    Message = "The Weather has been deleted successfully"
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


        public IEnumerable<Weather> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Weathers.Include(x => x.Value).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Value.Value.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "PeriodeType":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.PeriodeType)
                            : data.OrderByDescending(x => x.PeriodeType);
                        break;
                    case "Date":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Date)
                            : data.OrderByDescending(x => x.Date);
                        break;
                    case "Value":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Value)
                            : data.OrderByDescending(x => x.Value);
                        break;
                    case "Temperature":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Temperature)
                            : data.OrderByDescending(x => x.Temperature);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }



        public GetWeathersResponse GetWeathersForGrid(GetWeathersRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetWeathersResponse
            {
                TotalRecords = totalRecords,
                Weathers = data.ToList().MapTo<GetWeathersResponse.WeatherResponse>()
            };
        }
    }
}
