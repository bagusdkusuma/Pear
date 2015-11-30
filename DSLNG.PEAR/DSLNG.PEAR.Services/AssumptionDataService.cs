using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AssumptionData;
using DSLNG.PEAR.Services.Responses.AssumptionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class AssumptionDataService : BaseService, IAssumptionDataService
    {
        public AssumptionDataService(IDataContext context) : base(context) { }

        public GetAssumptionDatasResponse GetAssumptionDatas(GetAssumptionDatasRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary,request.ScenarioId, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetAssumptionDatasResponse
            {
                TotalRecords = totalRecords,
                AssumptionDatas = data.ToList().MapTo<GetAssumptionDatasResponse.AssumptionData>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetAssumptionDatasResponse { Count = DataContext.KeyAssumptionDatas.Count() };
            //}
            //else
            //{
            //    return new GetAssumptionDatasResponse
            //    {
            //        AssumptionDatas = DataContext.KeyAssumptionDatas.OrderByDescending(x => x.Id)
            //        .Include(x => x.Scenario).Include(x => x.KeyAssumptionConfig)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionDatasResponse.AssumptionData>()
            //    };
            //}
        }


        public GetAssumptionDataConfigResponse GetAssumptionDataConfig()
        {
            return new GetAssumptionDataConfigResponse
            {
                AssumptionDataConfigs = DataContext.KeyAssumptionConfigs.Include(x => x.Measurement).ToList().MapTo<GetAssumptionDataConfigResponse.AssumptionDataConfig>(),
                Scenarios = DataContext.Scenarios.ToList().MapTo<GetAssumptionDataConfigResponse.Scenario>()
            };

        }


        public SaveAssumptionDataResponse SaveAssumptionData(SaveAssumptionDataRequest request)
        {
            if (request.Id == 0)
            {
                var AssumptionData = DataContext.KeyAssumptionDatas.FirstOrDefault(x => x.Scenario.Id == request.IdScenario
                    && x.KeyAssumptionConfig.Id == request.IdConfig);
                if (AssumptionData == null)
                {
                    AssumptionData = request.MapTo<KeyAssumptionData>();
                    DataContext.KeyAssumptionDatas.Add(AssumptionData);
                }
                else {
                    var currentId = AssumptionData.Id;
                    request.MapPropertiesToInstance<KeyAssumptionData>(AssumptionData);
                    AssumptionData.Id = currentId;
                }
                AssumptionData.Scenario = DataContext.Scenarios.Where(x => x.Id == request.IdScenario).FirstOrDefault();
                AssumptionData.KeyAssumptionConfig = DataContext.KeyAssumptionConfigs.Where(x => x.Id == request.IdConfig).FirstOrDefault();
                
            }
            else
            {
                var AssumptionData = DataContext.KeyAssumptionDatas.Where(x => x.Id == request.Id).FirstOrDefault();
                if (AssumptionData != null)
                {
                    request.MapPropertiesToInstance<KeyAssumptionData>(AssumptionData);
                    AssumptionData.Scenario = DataContext.Scenarios.Where(x => x.Id == request.IdScenario).FirstOrDefault();
                    AssumptionData.KeyAssumptionConfig = DataContext.KeyAssumptionConfigs.Where(x => x.Id == request.IdConfig).FirstOrDefault();

                }
            }
            DataContext.SaveChanges();
            return new SaveAssumptionDataResponse
            {
                IsSuccess = true,
                Message = "Assumption Data has been Saved"
            };
        }


        public GetAssumptionDataResponse GetAssumptionData(GetAssumptionDataRequest request)
        {
            return DataContext.KeyAssumptionDatas.Where(x => x.Id == request.Id)
                .Include(x => x.Scenario).Include(x => x.KeyAssumptionConfig)
                .FirstOrDefault().MapTo<GetAssumptionDataResponse>();
        }



        public DeleteAssumptionDataResponse DeleteAssumptionData(DeleteAssumptionDataRequest request)
        {
            try
            {
                var assumptionData = DataContext.KeyAssumptionDatas.Where(x => x.Id == request.Id).FirstOrDefault();
                if (assumptionData != null)
                {
                    DataContext.KeyAssumptionDatas.Attach(assumptionData);
                    DataContext.KeyAssumptionDatas.Remove(assumptionData);
                    DataContext.SaveChanges();
                }
                return new DeleteAssumptionDataResponse
                {
                    IsSuccess = true,
                    Message = "Assumption Data has been deleted successfully"
                };
            }
            catch(DbUpdateException exception)
            {
                return new DeleteAssumptionDataResponse
                {
                    IsSuccess = false,
                    Message = exception.Message
                };
            }
        }


        public IEnumerable<KeyAssumptionData> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, int ScenarioId, out int TotalRecords)
        {
            var data = DataContext.KeyAssumptionDatas.Include(x => x.Scenario).Include(x => x.KeyAssumptionConfig).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.KeyAssumptionConfig.Name.Contains(search) || x.Scenario.Name.Contains(search));
            }
            if (ScenarioId != 0) {
                data = data.Where(x => x.Scenario.Id == ScenarioId);
            }
            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Scenario":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Scenario.Name)
                            : data.OrderByDescending(x => x.Scenario.Name);
                        break;
                    case "Config":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.KeyAssumptionConfig.Name)
                            : data.OrderByDescending(x => x.KeyAssumptionConfig.Name);
                        break;
                    case "ActualValue":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ActualValue)
                            : data.OrderByDescending(x => x.ActualValue);
                        break;
                    case "ForecastValue":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ForecastValue)
                            : data.OrderByDescending(x => x.ForecastValue);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }
    }
}
