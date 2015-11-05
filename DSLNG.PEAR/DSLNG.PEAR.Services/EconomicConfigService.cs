using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EconomicConfig;
using DSLNG.PEAR.Services.Responses.EconomicConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;

namespace DSLNG.PEAR.Services
{
    public class EconomicConfigService : BaseService, IEconomicConfigService
    {
        public EconomicConfigService(IDataContext context) : base(context) { }


        public GetEconomicConfigsResponse GetEconomicConfigs(GetEconomicConfigsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetEconomicConfigsResponse { Count = DataContext.EconomicConfigDetails.Count() };
            }
            else
            {
                return new GetEconomicConfigsResponse
                {
                    EconomicConfigs = DataContext.EconomicConfigDetails.OrderByDescending(x => x.Id)
                    .Include(x => x.Scenario).Include(x => x.EconomicSummary).ToList().MapTo<GetEconomicConfigsResponse.EconomicConfig>()
                };
            }

        }


        public GetEconomicConfigSelectListResponse GetEconomicConfigSelectList()
        {
            return new GetEconomicConfigSelectListResponse
            {
                Scenarios = DataContext.Scenarios.ToList().MapTo<GetEconomicConfigSelectListResponse.Scenario>(),
                EconomicSummaries = DataContext.EconomicSummaryConfigs.ToList().MapTo<GetEconomicConfigSelectListResponse.EconomicSummary>()
            };
        }


        public SaveEconomicConfigResponse SaveEconomicConfig(SaveEconomicConfigRequest request)
        {
            if (request.Id == 0)
            {
                var Economic = request.MapTo<EconomicConfigDetail>();
                Economic.Scenario = DataContext.Scenarios.FirstOrDefault(x => x.Id == request.IdScenario);
                Economic.EconomicSummary = DataContext.EconomicSummaryConfigs.FirstOrDefault(x => x.Id == request.IdEconomicSummary);
                DataContext.EconomicConfigDetails.Add(Economic);
            }
            else
            {
                var Economic = DataContext.EconomicConfigDetails.FirstOrDefault(x => x.Id == request.Id);
                if (Economic != null)
                {
                    request.MapPropertiesToInstance<EconomicConfigDetail>(Economic);
                    Economic.Scenario = DataContext.Scenarios.FirstOrDefault(x => x.Id == request.IdScenario);
                    Economic.EconomicSummary = DataContext.EconomicSummaryConfigs.FirstOrDefault(x => x.Id == request.IdEconomicSummary);
                }
            }
            DataContext.SaveChanges();
            return new SaveEconomicConfigResponse
            {
                IsSuccess = true,
                Message = "Economic Config has been Save"
            };
        }


        public GetEconomicConfigResponse GetEconomicConfig(GetEconomicConfigRequest request)
        {
            return DataContext.EconomicConfigDetails
                .Include(x => x.Scenario).Include(x => x.EconomicSummary)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetEconomicConfigResponse>();
        }


        public DeleteEconomicConfigResponse DeleteEconomicConfig(DeleteEconomicConfigRequest request)
        {
            var CheckId = DataContext.EconomicConfigDetails.FirstOrDefault(x => x.Id == request.Id);
            if (CheckId != null)
            {
                DataContext.EconomicConfigDetails.Attach(CheckId);
                DataContext.EconomicConfigDetails.Remove(CheckId);
                DataContext.SaveChanges();
            }
            return new DeleteEconomicConfigResponse
            {
                IsSuccess = true,
                Message = "Economic Config has been deleted successfully"
            };
        }
    }
}
