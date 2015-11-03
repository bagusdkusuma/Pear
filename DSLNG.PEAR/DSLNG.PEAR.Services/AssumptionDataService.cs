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

namespace DSLNG.PEAR.Services
{
    public class AssumptionDataService : BaseService, IAssumptionDataService
    {
        public AssumptionDataService(IDataContext context) : base(context) { }

        public GetAssumptionDatasResponse GetAssumptionDatas(GetAssumptionDatasRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetAssumptionDatasResponse { Count = DataContext.KeyAssumptionDatas.Count() };
            }
            else
            {
                return new GetAssumptionDatasResponse
                {
                    AssumptionDatas = DataContext.KeyAssumptionDatas.OrderByDescending(x => x.Id)
                    .Include(x => x.Scenario).Include(x => x.KeyAssumptionConfig)
                    .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionDatasResponse.AssumptionData>()
                };
            }
        }


        public GetAssumptionDataConfigResponse GetAssumptionDataConfig()
        {
            return new GetAssumptionDataConfigResponse
            {
                AssumptionDataConfigs = DataContext.KeyAssumptionConfigs.ToList().MapTo<GetAssumptionDataConfigResponse.AssumptionDataConfig>(),
                Scenarios = DataContext.Scenarios.ToList().MapTo<GetAssumptionDataConfigResponse.Scenario>()
            };

        }


        public SaveAssumptionDataResponse SaveAssumptionData(SaveAssumptionDataRequest request)
        {
            if (request.Id == 0)
            {
                var AssumptionData = request.MapTo<KeyAssumptionData>();
                AssumptionData.Scenario = DataContext.Scenarios.Where(x => x.Id == request.IdScenario).FirstOrDefault();
                AssumptionData.KeyAssumptionConfig = DataContext.KeyAssumptionConfigs.Where(x => x.Id == request.IdConfig).FirstOrDefault();
                DataContext.KeyAssumptionDatas.Add(AssumptionData);
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
                Message = "Assumption Data has been Save"
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
            var checkid = DataContext.KeyAssumptionDatas.Where(x => x.Id == request.Id).FirstOrDefault();
            if (checkid != null)
            {
                DataContext.KeyAssumptionDatas.Attach(checkid);
                DataContext.KeyAssumptionDatas.Remove(checkid);
                DataContext.SaveChanges();
            }
            return new DeleteAssumptionDataResponse
            {
                IsSuccess = true,
                Message = "Assumption Data has been Delete"
            };
        }
    }
}
