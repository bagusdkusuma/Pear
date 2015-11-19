

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Weather
{
    public class GetWeathersResponse
    {
        public IList<WeatherResponse> Weathers { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class WeatherResponse
        {
            public int Id { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string Value { get; set; }
            public DateTime Date { get; set; }
            public string Temperature { get; set; }
        }
    }
}
