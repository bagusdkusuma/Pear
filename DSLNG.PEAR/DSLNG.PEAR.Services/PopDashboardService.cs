using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PopDashboard;
using DSLNG.PEAR.Services.Responses.PopDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Pop;
using System.Data.SqlClient;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class PopDashboardService: BaseService, IPopDashboardService
    {
        public PopDashboardService(IDataContext dataContext) : base(dataContext){}



        public GetPopDashboardsResponse GetPopDashboards(GetPopDashboardsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetPopDashboardsResponse
            {
                TotalRecords = totalRecords,
                PopDashboards = data.ToList().MapTo<GetPopDashboardsResponse.PopDashboard>()
            };


        }

        public IEnumerable<PopDashboard> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.PopDashboards.Include(x => x.PopInformations).Include(x => x.Signatures).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Number.Contains(search) || x.Subtitle.Contains(search) || x.Title.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Title":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Title).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Title).ThenBy(x => x.IsActive);
                        break;
                    case "Subtitle":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Subtitle).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Subtitle).ThenBy(x => x.IsActive);
                        break;
                    case "Number":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Number).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Number).ThenBy(x => x.IsActive);
                        break;
                    case "Status":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Status).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Status).ThenBy(x => x.IsActive);
                        break;
                    case "StructureOwner":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.StructureOwner).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.StructureOwner).ThenBy(x => x.IsActive);
                        break;
                    case "ResourceTotalCost":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ResourceTotalCost).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.ResourceTotalCost).ThenBy(x => x.IsActive);
                        break;
                }
            }


            TotalRecords = data.Count();
            return data;
        }



        public SavePopDashboardResponse SavePopDashboard(SavePopDashboardRequest request)
        {
            var popDashboard = request.MapTo<PopDashboard>();
            if (request.Id == 0)
            {
                DataContext.PopDashboards.Add(popDashboard);
            }
            else
            {
                popDashboard = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.Id);
                request.MapPropertiesToInstance<PopDashboard>(popDashboard);
            }

            DataContext.SaveChanges();
            return new SavePopDashboardResponse
            {
                IsSuccess = true,
                Message = "Pop Dashboard has been saved successfully!"
            };
        }



        public GetPopDashboardResponse GetPopDashboard(GetPopDashboardRequest request)
        {
            return DataContext.PopDashboards.Where(x => x.Id == request.Id)
                .Include(x => x.PopInformations)
                .Include(x => x.Signatures)
                .Include(x => x.Signatures.Select(y => y.User))
                .FirstOrDefault().MapTo<GetPopDashboardResponse>();
        }


        public DeletePopDashboardResponse DeletePopDashboard(int request)
        {
            var popDashboard = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request);
            DataContext.PopDashboards.Attach(popDashboard);
            DataContext.PopDashboards.Remove(popDashboard);
            DataContext.SaveChanges();
            return new DeletePopDashboardResponse
            {
                IsSuccess = true,
                Message = "Pop Dashboard has been Deleted!"
            };
        }
    }
}
