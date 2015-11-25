

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Collections;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services
{
    public class OutputConfigService : BaseService,IOutputConfigService
    {

        public OutputConfigService(IDataContext dataContext) : base(dataContext) { }

        public GetKpisResponse GetKpis(GetKpisRequest request)
        {
            return new GetKpisResponse
            {
                KpiList = DataContext.Kpis.Where(x => x.Name.Contains(request.Term) && x.IsEconomic == true).Take(20).ToList().MapTo<GetKpisResponse.Kpi>()
            };
        }


        public GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request)
        {
            return new GetKeyAssumptionsResponse
            {
                KeyAssumptions = DataContext.KeyAssumptionConfigs.Where(x => x.Name.Contains(request.Term)).Take(20).ToList().MapTo<GetKeyAssumptionsResponse.KeyAssumption>()
            };
        }


        public GetOutputConfigsResponse GetOutputConfigs(GetOutputConfigsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOutputConfigsResponse
            {
                TotalRecords = totalRecords,
                OutputConfigs = data.ToList().MapTo<GetOutputConfigsResponse.OutputConfig>()
            };
        }


        public IEnumerable<KeyOutputConfiguration> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOutputConfigs.Include(x => x.Category).Include(x => x.Measurement).Include(x => x.KeyAssumptions).Include(x => x.Kpis).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Category.Name.Contains(search) || x.Measurement.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                        break;
                    case "Category":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Category.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Category.Name).ThenBy(x => x.Order);
                        break;
                    case "Measurement":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Measurement.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Measurement.Name).ThenBy(x => x.Order);
                        break;
                    case "Formula":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Formula).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Formula).ThenBy(x => x.Order);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Order)
                            : data.OrderByDescending(x => x.Order);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }
    }
}
