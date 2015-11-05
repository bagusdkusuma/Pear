
using DSLNG.PEAR.Data.Enums;
using System;
namespace DSLNG.PEAR.Services.Responses.Weather
{
    public class GetWeatherResponse
    {
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public int ValueId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Temperature { get; set; }
    }
}
