

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Operation
{
    public class GetOperationsInResponse
    {
        public IList<KeyOperationResponse> KeyOperations { get; set; }
        
        public class KeyOperationResponse
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
        }
    }
}
