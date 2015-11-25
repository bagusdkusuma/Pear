



using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationalDataDetailResponse
    {
        public GetOperationalDataDetailResponse()
        {
            KeyOperationGroups = new List<KeyOperationGroup>();
        }
        public IList<KeyOperationGroup> KeyOperationGroups { get; set; }

        public class KeyOperationGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
            public IList<KeyOperationConfig> KeyOperationConfigs { get; set; }
        }

        public class KeyOperationConfig
        {
            public int Id { get; set; }
            public Kpi Kpi { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public int Order { get; set; }
        }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string MeasurementName { get; set; }
        }
    }
}
