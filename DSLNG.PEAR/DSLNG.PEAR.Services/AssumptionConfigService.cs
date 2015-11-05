using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AssumptionConfig;
using DSLNG.PEAR.Services.Responses.AssumptionConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Entities.EconomicModel;

namespace DSLNG.PEAR.Services
{
    public class AssumptionConfigService : BaseService, IAssumptionConfigService
    {
        public AssumptionConfigService(IDataContext context) : base(context) { }

        public GetAssumptionConfigsResponse GetAssumptionConfigs(GetAssumptionConfigsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetAssumptionConfigsResponse { Count = DataContext.KeyAssumptionConfigs.Count() };
            }
            else
            {
                return new GetAssumptionConfigsResponse
                {
                    AssumptionConfigs = DataContext.KeyAssumptionConfigs.OrderByDescending(x => x.Id)
                    .Include(x => x.Category).Include(y => y.Measurement)
                    .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionConfigsResponse.AssumptionConfig>()
                };
            }
        }


        public GetAssumptionConfigCategoryResponse GetAssumptionConfigCategories()
        {
            return new GetAssumptionConfigCategoryResponse
            {
                AssumptionConfigCategoriesResponse = DataContext.KeyAssumptionCategories.ToList().MapTo<GetAssumptionConfigCategoryResponse.AssumptionConfigCategoryResponse>()
            };

        }


        public SaveAssumptionConfigResponse SaveAssumptionConfig(SaveAssumptionConfigRequest request)
        {
            if (request.Id == 0)
            {
                var AssumptionConfig = request.MapTo<KeyAssumptionConfig>();
                AssumptionConfig.Category = DataContext.KeyAssumptionCategories.Where(x => x.Id == request.IdCategory).FirstOrDefault();
                AssumptionConfig.Measurement = DataContext.Measurements.Where(x => x.Id == request.IdMeasurement).FirstOrDefault();
                DataContext.KeyAssumptionConfigs.Add(AssumptionConfig);                

            }
            else
            {
                var AssumptionConfig = DataContext.KeyAssumptionConfigs.FirstOrDefault(x => x.Id == request.Id);
                if (AssumptionConfig != null)
                {
                    request.MapPropertiesToInstance<KeyAssumptionConfig>(AssumptionConfig);
                    AssumptionConfig.Category = DataContext.KeyAssumptionCategories.Where(x => x.Id == request.IdCategory).FirstOrDefault();
                    AssumptionConfig.Measurement = DataContext.Measurements.Where(x => x.Id == request.IdMeasurement).FirstOrDefault();
                }
                
            }
            DataContext.SaveChanges();
            return new SaveAssumptionConfigResponse
            {
                IsSuccess = true,
                Message = "Assumption Config has been saved"
            };
        }


        public GetAssumptionConfigResponse GetAssumptionConfig(GetAssumptionConfigRequest request)
        {
            var respone = DataContext.KeyAssumptionConfigs
                .Include(x => x.Category)
                .Include(x => x.Measurement)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetAssumptionConfigResponse>();
            return respone;

        }


        public DeleteAssumptionConfigResponse DeleteAssumptionConfig(DeleteAssumptionConfigRequest request)
        {
            var AssumptionConfig = new KeyAssumptionConfig();
            AssumptionConfig.Id = request.Id;
            DataContext.Entry(AssumptionConfig).State = EntityState.Deleted;
            DataContext.SaveChanges();
            return new DeleteAssumptionConfigResponse
            {
                IsSuccess = true,
                Message = "The Assumption Category has been deleted successfully"
            };
        }
    }
}
