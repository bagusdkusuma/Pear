

using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightGroup;
using DSLNG.PEAR.Services.Responses.HighlightGroup;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Persistence;

namespace DSLNG.PEAR.Services
{
    public class HighlightGroupService : BaseService, IHighlightGroupService
    {
        public HighlightGroupService(IDataContext context) : base(context) { }

        public GetHighlightGroupsResponse GetHighlightGroups(GetHighlightGroupsRequest request)
        {
            int totalRecords;
            var query = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                query = query.Skip(request.Skip).Take(request.Take);
            }
            var artifacts = query.ToList();

            var response = new GetHighlightGroupsResponse();
            response.HighlightGroups = artifacts.MapTo<GetHighlightGroupsResponse.HighlightGroupResponse>();
            response.TotalRecords = totalRecords;

            return response;
        }
        private IEnumerable<HighlightGroup> SortData(string search, IDictionary<string, System.Data.SqlClient.SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.HighlightGroups.Include(x => x.Options).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Name)
                                   : data.OrderByDescending(x => x.Name);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Order);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }
        public GetHighlightGroupResponse GetHighlightGroup(GetHighlightGroupRequest request)
        {
            var highlightGroup = DataContext.HighlightGroups.FirstOrDefault(x => x.Id == request.Id);
            return highlightGroup.MapTo<GetHighlightGroupResponse>();
        }

        public SaveHighlightGroupResponse Save(SaveHighlightGroupRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var highlightGroup = request.MapTo<HighlightGroup>();
                    DataContext.HighlightGroups.Add(highlightGroup);
                }
                else {
                    var higlightGroup = DataContext.HighlightGroups.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<HighlightGroup>(higlightGroup);
                }
                DataContext.SaveChanges();
                return new SaveHighlightGroupResponse
                {
                    IsSuccess = true, 
                    Message = "You have been successfully deleted the item"
                };
            }
            catch {
                return new SaveHighlightGroupResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }
    }
}
