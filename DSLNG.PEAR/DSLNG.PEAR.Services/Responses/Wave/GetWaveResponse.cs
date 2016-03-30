using DSLNG.PEAR.Data.Enums;
using System;

namespace DSLNG.PEAR.Services.Responses.Wave
{
    public class GetWaveResponse
    {
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public int ValueId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Speed { get; set; }
        public string Tide { get; set; }
    }
}
