
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Highlight;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using DSLNG.PEAR.Data.Enums;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class HighlightService : BaseService, IHighlightService
    {
        public HighlightService(IDataContext context) : base(context) { }
        public GetHighlightsResponse GetHighlights(GetHighlightsRequest request)
        {
            if (request.OnlyCount)
            {
                var query = DataContext.Highlights.Include(x => x.HighlightType).AsQueryable();
                if (request.PeriodeType != null)
                {
                    query = query.Where(x => x.PeriodeType == request.PeriodeType);
                }
                return new GetHighlightsResponse { Count = query.Count() };
            }
            else if (request.Except.Length > 0 && request.Date.HasValue)
            {
                return new GetHighlightsResponse
                {
                    Highlights = DataContext.Highlights.Include(x => x.HighlightType).Where(x => x.Date == request.Date.Value && !request.Except.Contains(x.HighlightType.Value) && x.IsActive == request.IsActive)
                    .ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
                };
            }
            else if (request.Include.Length > 0 && request.Date.HasValue)
            {
                return new GetHighlightsResponse
                {
                    Highlights = DataContext.Highlights.Include(x => x.HighlightType).Where(x => x.Date == request.Date.Value && request.Include.Contains(x.HighlightType.Value) && x.IsActive == request.IsActive)
                    .ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
                };
            }
            else
            {
                var query = DataContext.Highlights.Include(x => x.HighlightType).AsQueryable();
                if (request.PeriodeType == request.PeriodeType) // WTF????
                {
                    query = query.Where(x => x.PeriodeType == request.PeriodeType);
                }
                return new GetHighlightsResponse
                {
                    Highlights = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take)
                                    .ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
                };
            }
        }

        public GetHighlightResponse GetHighlightByPeriode(GetHighlightRequest request, bool latest = false)
        {
            
            var data = DataContext.Highlights
                                  .Include(x => x.HighlightType)
                                  .Where(x => x.PeriodeType == (request.PeriodeType.HasValue? request.PeriodeType : PeriodeType.Daily))
                                  .AsQueryable();
            if (latest)
            {
                data = data.OrderByDescending(x => x.Date);
            }
            else {
                data = data.Where(x => x.Date == request.Date);
            }
            

            if (request.HighlightTypeId > 0)
            {
                data = data.Where(x => x.HighlightType.Id == request.HighlightTypeId);
            } 

            if (!string.IsNullOrEmpty(request.Type))
            {
                data = data.Where(x => x.Type == request.Type);
            }

            var result = data.FirstOrDefault();
            if (result != null)
            {
                return result.MapTo<GetHighlightResponse>();
            }

            return new GetHighlightResponse();
        }

        public SaveHighlightResponse SaveHighlight(SaveHighlightRequest request)
        {
            try
            {
                var action = request.MapTo<BaseAction>();
                var todayHighlight = DataContext.Highlights.FirstOrDefault(x =>x.PeriodeType == request.PeriodeType && x.Date == request.Date && x.HighlightType.Id == request.TypeId);
                if (todayHighlight != null && todayHighlight.Id != request.Id)
                {
                    return new SaveHighlightResponse
                    {
                        IsSuccess = false,
                        Message = "You can only save one type of highlight in the same periode of time"
                    };
                }
                var highlight = request.MapTo<Highlight>();
                if (request.Id == 0)
                {
                    
                    var selectOption = DataContext.SelectOptions.Single(x => x.Id == request.TypeId);
                    var highlightType = selectOption;
                    highlight.Type = selectOption.Value;
                    DataContext.SelectOptions.Attach(highlightType);
                    highlight.HighlightType = highlightType;
                    DataContext.Highlights.Add(highlight);
                }
                else
                {
                    highlight = DataContext.Highlights.FirstOrDefault(x => x.Id == request.Id);
                    if (highlight != null)
                    {
                        request.MapPropertiesToInstance<Highlight>(highlight);
                        var selectOption = DataContext.SelectOptions.Single(x => x.Id == request.TypeId);
                        var highlightType = selectOption;
                        highlight.Type = selectOption.Value;
                        DataContext.SelectOptions.Attach(highlightType);
                        highlight.HighlightType = highlightType;
                    }
                }
                DataContext.SaveChanges(action);
                return new SaveHighlightResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved",
                    Id = highlight.Id
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveHighlightResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to save highlight data, Error message = " + e.Message + " and ID = " + request.Id
                };
            }
            catch (Exception e)
            {
                return new SaveHighlightResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to save highlight data, Error message = " + e.Message + " and ID = " + request.Id
                };
            }
        }
        public GetReportHighlightsResponse GetReportHighlights(GetReportHighlightsRequest request)
        {
            var response = new GetReportHighlightsResponse();
            var start = request.TimePeriodes.First();
            var end = request.TimePeriodes[request.TimePeriodes.Count - 1];
            var highlights = DataContext.Highlights.Where(x => x.Date >= start && x.Date <= end
                 && x.HighlightType.Value == request.Type
                 && x.PeriodeType == request.PeriodeType).ToList();
            var reportHighlights = new List<GetReportHighlightsResponse.HighlightResponse>();
            foreach (var time in request.TimePeriodes)
            {
                var highlight = highlights.FirstOrDefault(x => x.Date == time);
                if (highlight != null)
                {
                    reportHighlights.Add(new GetReportHighlightsResponse.HighlightResponse
                    {
                        Title = highlight.Title,
                        Message = highlight.Message
                    });
                }
                else
                {
                    reportHighlights.Add(null);
                }
            }
            response.Highlights = reportHighlights;
            return response;
        }


        public GetHighlightResponse GetHighlight(GetHighlightRequest request)
        {
            if (request.Id != 0)
            {
                return DataContext.Highlights.Include(x => x.HighlightType).FirstOrDefault(x => x.Id == request.Id).MapTo<GetHighlightResponse>();
            }
            else
            {
                var highlightQuery = DataContext.Highlights.Include(x => x.HighlightType);
                if (request.Date.HasValue)
                {
                    highlightQuery = highlightQuery.Where(x => x.Date == request.Date && x.HighlightType.Value == request.Type);
                }
                else
                {
                    highlightQuery = highlightQuery.OrderByDescending(x => x.Date).Where(x => x.HighlightType.Value == request.Type);
                }
                var highlight = highlightQuery.FirstOrDefault();
                if (highlight != null)
                {
                    return highlight.MapTo<GetHighlightResponse>();
                }
                return new GetHighlightResponse();
            }
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
            catch (InvalidOperationException)
            {
                return new DeleteResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this highlight"
                };
            }
        }


        public GetDynamicHighlightsResponse GetDynamicHighlights(GetDynamicHighlightsRequest request)
        {
            var latestHighlight = DataContext.Highlights.OrderByDescending(x => x.Date)
                .FirstOrDefault(x => x.PeriodeType == request.PeriodeType && !x.HighlightType.Text.Equals("Alert", StringComparison.InvariantCultureIgnoreCase));
            if (request.Periode != null)
            {
                latestHighlight = DataContext.Highlights.FirstOrDefault(x => x.PeriodeType == request.PeriodeType && x.Date == request.Periode);
            }
            var highlights = new List<Highlight>();
            if (latestHighlight != null)
            {
                switch (request.PeriodeType)
                {

                    case PeriodeType.Monthly:
                        highlights = DataContext.Highlights.Include(x => x.HighlightType)
                            .Include(x => x.HighlightType.Group)
                            .Where(x => x.Date.Month == latestHighlight.Date.Month && x.Date.Year == latestHighlight.Date.Year
                                && x.PeriodeType == request.PeriodeType && x.HighlightType.Group != null).ToList();
                        break;
                    case PeriodeType.Yearly:
                        highlights = DataContext.Highlights.Include(x => x.HighlightType)
                           .Include(x => x.HighlightType.Group)
                           .Where(x => x.Date.Year == latestHighlight.Date.Year
                               && x.PeriodeType == request.PeriodeType && x.HighlightType.Group != null).ToList();
                        break;
                    default:
                        highlights = DataContext.Highlights.Include(x => x.HighlightType)
                           .Include(x => x.HighlightType.Group)
                           .Where(x => x.Date == latestHighlight.Date
                               && x.PeriodeType == request.PeriodeType && x.HighlightType.Group != null).ToList();
                        break;
                }
            }
            if (highlights.Count > 0)
            {
                return new GetDynamicHighlightsResponse
                {
                    Periode = latestHighlight.Date,
                    HighlightGroups = highlights.GroupBy(x => x.HighlightType.Group,
                       x => x,
                        (key, g) => new GetDynamicHighlightsResponse.HighlightGroupResponse
                        {
                            Id = key.Id,
                            Name = key.Name,
                            Order = key.Order,
                            Highlights = g.ToList().MapTo<GetDynamicHighlightsResponse.HighlightResponse>()
                        }).ToList()
                };
            }
            return new GetDynamicHighlightsResponse();
        }


        public IEnumerable<Highlight> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Highlights.Include(x => x.HighlightType).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Type.Contains(search) || x.Title.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "PeriodeType":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.PeriodeType)
                            : data.OrderByDescending(x => x.PeriodeType);
                        break;
                    case "Type":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Type)
                            : data.OrderByDescending(x => x.Type);
                        break;
                    case "Title":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Title)
                            : data.OrderByDescending(x => x.Title);
                        break;
                    case "Date":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Date)
                            : data.OrderByDescending(x => x.Date);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
        }


        public GetHighlightsResponse GetHighlightsForGrid(GetHighlightsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetHighlightsResponse
            {
                TotalRecords = totalRecords,
                Highlights = data.ToList().MapTo<GetHighlightsResponse.HighlightResponse>()
            };
        }
    }
}
