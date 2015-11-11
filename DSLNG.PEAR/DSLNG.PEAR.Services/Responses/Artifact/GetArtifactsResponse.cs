
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class GetArtifactsResponse : BaseResponse
    {
        public IList<Artifact> Artifacts { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class Artifact {
            public int Id { get; set; }
            public string GraphicType { get; set; }
            public string GraphicName { get; set; }
            public bool Used { get; set; }
        }
    }
}
