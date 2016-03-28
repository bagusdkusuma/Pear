

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Wave
{
    public class GetWavesResponse
    {
        public IList<WaveResponse> Waves { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }

        public class WaveResponse
        {
            public int Id { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string Value { get; set; }
            public DateTime Date { get; set; }
            public string Speed { get; set; }
            public string Tide { get; set; }
        }
    }
}
