
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Highlight;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;

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
            else if (request.Except.Length > 0 && request.Date.HasValue) {
                return new GetHighlightsResponse
                {
                    Highlights = DataContext.Highlights.Where(x => x.Date == request.Date.Value && !request.Except.Contains(x.Type))
                    .ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
                };
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
        public SaveHighlightResponse SaveHighlight(SaveHighlightRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var highlight = request.MapTo<Highlight>();
                    DataContext.Highlights.Add(highlight);
                }
                else
                {
                    var highlight = DataContext.Highlights.FirstOrDefault(x => x.Id == request.Id);
                    if (highlight != null)
                    {
                        request.MapPropertiesToInstance<Highlight>(highlight);
                    }
                }
                DataContext.SaveChanges();
                return new SaveHighlightResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveHighlightResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                }; 
            }
      
        }
        public GetReportHighlightsResponse GetReportHighlights(GetReportHighlightsRequest request) {
            var response = new GetReportHighlightsResponse();
            var start = request.TimePeriodes.First();
            var end = request.TimePeriodes[request.TimePeriodes.Count - 1];
            var highlights = DataContext.Highlights.Where(x => x.Date >= start && x.Date <= end
                 && x.Type == request.Type
                 && x.PeriodeType == request.PeriodeType).ToList();
            var reportHighlights = new List<GetReportHighlightsResponse.HighlightResponse>();
            foreach (var time in request.TimePeriodes) {
                var highlight = highlights.FirstOrDefault(x => x.Date == time);
                if (highlight != null)
                {
                    reportHighlights.Add(new GetReportHighlightsResponse.HighlightResponse {
                        Title = highlight.Title,
                        Message = highlight.Message
                    });
                }
                else {
                    reportHighlights.Add(null);
                }
            }
            response.Highlights = reportHighlights;
            return response;
        }


        public GetHighlightResponse GetHighlight(GetHighlightRequest request)
        {
            return DataContext.Highlights.FirstOrDefault(x => x.Id == request.Id).MapTo<GetHighlightResponse>();
        }


        public DeleteResponse DeleteHighlight(DeleteRequest request)
        {
            try
            {
                var highlight = new Highlight { Id = request.Id };
                DataContext.Highlights.Attach(highlight);
                DataContext.Highlights.Remove(highlight);
                DataContext.SaveChanges();
                return new DeleteResponse
                {
                    IsSuccess = true,
                    Message = "The highlight has been deleted successfully"
                };
            }
            catch (InvalidOperationException) {
                return new DeleteResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this highlight"
                };
            }
        }
    }
}
