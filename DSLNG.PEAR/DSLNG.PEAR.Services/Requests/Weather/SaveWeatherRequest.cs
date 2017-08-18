

using DSLNG.PEAR.Data.Enums;
using System;
namespace DSLNG.PEAR.Services.Requests.Weather
{
    public class SaveWeatherRequest : BaseRequest
    {
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public int ValueId { get; set; }
        public string Temperature { get; set; }
    }
}
