
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Highlight;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;

namespace DSLNG.PEAR.Services
{
    public class HighlightService  : BaseService,IHighlightService
    {
        public HighlightService(IDataContext context) : base(context) { }
        public GetHighlightsResponse GetHighlights(GetHighlightsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetHighlightsResponse { Count = DataContext.Highlights.Count() };
            }
            else
            {
                return new GetHighlightsResponse
                {
                    Highlights = DataContext.Highlights.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take)
                                    .ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
                };
            }
        }
    }
}
