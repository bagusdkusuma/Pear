using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EconomicSummary;
using DSLNG.PEAR.Services.Responses.EconomicSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class EconomicSummaryService : BaseService, IEconomicSummaryService
    {
        public EconomicSummaryService(IDataContext context) : base(context) { }

        public GetEconomicSummariesResponse GetEconomicSummaries(GetEconomicSummariesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetEconomicSummariesResponse
            {
                TotalRecords = totalRecords,
                EconomicSummaries = data.ToList().MapTo<GetEconomicSummariesResponse.EconomicSummary>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetEconomicSummariesResponse { Count = DataContext.EconomicSummaryConfigs.Count() };
            //}
            //else
            //{
            //    return new GetEconomicSummariesResponse
            //    {
            //        EconomicSummaries = DataContext.EconomicSummaryConfigs.OrderByDescending(x => x.Id)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetEconomicSummariesResponse.EconomicSummary>()
            //    };
            //}
        }


        public SaveEconomicSummaryResponse SaveEconomicSummary(SaveEconomicSummaryRequest request)
        {
            if (request.Id == 0)
            {
                var Economic = request.MapTo<EconomicSummaryConfig>();
                DataContext.EconomicSummaryConfigs.Add(Economic);
            }
            else
            {
                var Economic = DataContext.EconomicSummaryConfigs.FirstOrDefault(x => x.Id == request.Id);
                if (Economic != null)
                {
                    request.MapPropertiesToInstance<EconomicSummaryConfig>(Economic);
                }
            }
            DataContext.SaveChanges();

            return new SaveEconomicSummaryResponse
            {
                IsSuccess = true,
                Message = "Economic Summary Config has been Save"
            };
        }


        public GetEconomicSummaryResponse GetEconomicSummary(GetEconomicSummaryRequest request)
        {
            return DataContext.EconomicSummaryConfigs.FirstOrDefault(x => x.Id == request.Id).MapTo<GetEconomicSummaryResponse>();
        }


        public DeleteEconomicSummaryResponse DeleteEconomicSummary(DeleteEconomicSummaryRequest request)
        {
            var checkId = DataContext.EconomicSummaryConfigs.FirstOrDefault(x => x.Id == request.Id);
            if (checkId != null)
            {
                DataContext.EconomicSummaryConfigs.Attach(checkId);
                DataContext.EconomicSummaryConfigs.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteEconomicSummaryResponse
            {
                IsSuccess = true,
                Message = "Economic Summary has deleted successfully"
            };
        }

        public IEnumerable<EconomicSummaryConfig> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.EconomicSummaryConfigs.AsQueryable();
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
                    case "Desc":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Desc)
                            : data.OrderByDescending(x => x.Desc);
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
    }
}
