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
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class AssumptionConfigService : BaseService, IAssumptionConfigService
    {
        public AssumptionConfigService(IDataContext context) : base(context) { }

        public GetAssumptionConfigsResponse GetAssumptionConfigs(GetAssumptionConfigsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetAssumptionConfigsResponse
            {
                TotalRecords = totalRecords,
                AssumptionConfigs = data.ToList().MapTo<GetAssumptionConfigsResponse.AssumptionConfig>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetAssumptionConfigsResponse { Count = DataContext.KeyAssumptionConfigs.Count() };
            //}
            //else
            //{
            //    return new GetAssumptionConfigsResponse
            //    {
            //        AssumptionConfigs = DataContext.KeyAssumptionConfigs.OrderByDescending(x => x.Id)
            //        .Include(x => x.Category).Include(y => y.Measurement)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionConfigsResponse.AssumptionConfig>()
            //    };
            //}
        }


        public GetAssumptionConfigCategoryResponse GetAssumptionConfigCategories()
        {
            return new GetAssumptionConfigCategoryResponse
            {
                AssumptionConfigCategoriesResponse = DataContext.KeyAssumptionCategories.ToList().MapTo<GetAssumptionConfigCategoryResponse.AssumptionConfigCategoryResponse>(),
                MeasurementsSelectList = DataContext.Measurements.ToList().MapTo < GetAssumptionConfigCategoryResponse.MeasurementSelectList>()
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
                var AssumptionConfig = DataContext.KeyAssumptionConfigs.Include(x => x.Measurement)
                    .Include(x => x.Category).FirstOrDefault(x => x.Id == request.Id);
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
            try
            {
                var assumptionConfig = new KeyAssumptionConfig { Id = request.Id };
                DataContext.KeyAssumptionConfigs.Attach(assumptionConfig);
                DataContext.KeyAssumptionConfigs.Remove(assumptionConfig);
                DataContext.SaveChanges();

                return new DeleteAssumptionConfigResponse
                {
                    IsSuccess = true,
                    Message = "The Assumption Config has been deleted successfully"
                };
            }
            catch(DbUpdateException exception)
            {
                return new DeleteAssumptionConfigResponse
                {
                    IsSuccess = false,
                    Message = exception.Message
                };
            }

        }


        public IEnumerable<KeyAssumptionConfig> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyAssumptionConfigs.Include(x => x.Category).Include(x => x.Measurement).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Category.Name.Contains(search)
                    || x.Measurement.Name.Contains(search));
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
