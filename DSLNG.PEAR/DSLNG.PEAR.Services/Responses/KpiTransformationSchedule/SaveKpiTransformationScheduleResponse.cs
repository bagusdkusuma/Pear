
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.KpiTransformationSchedule
{
    public class SaveKpiTransformationScheduleResponse : BaseResponse
    {
        public int Id { get; set; }
        public int KpiTransformationId { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public IList<KpiResponse> SelectedKpis { get; set; }
        public DateTime ProcessingDate { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public KpiTransformationStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public class KpiResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public YtdFormula YtdFormula { get; set; }
            public string CustomFormula { get; set; }
            public int MethodId { get; set; }
        }
    }
}
