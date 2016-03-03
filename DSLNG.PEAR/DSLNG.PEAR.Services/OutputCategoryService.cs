using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.OutputCategory;
using DSLNG.PEAR.Services.Responses.OutputCategory;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class OutputCategoryService : BaseService, IOutputCategoryService
    {

        public OutputCategoryService(IDataContext context) : base(context) { }

        GetOutputCategoriesResponse IOutputCategoryService.GetOutputCategories(GetOutputCategoriesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOutputCategoriesResponse
            {
                TotalRecords = totalRecords,
                OutputCategories = data.ToList().MapTo<GetOutputCategoriesResponse.OutputCategory>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetOutputCategoriesResponse { Count = DataContext.KeyOutputCategories.Count() };
            //}
            //else
            //{
            //    return new GetOutputCategoriesResponse
            //    {
            //        OutputCategories = DataContext.KeyOutputCategories.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOutputCategoriesResponse.OutputCategory>()
            //    };
            //}
        }


        public SaveOutputCategoryRespone SaveOutputCategory(SaveOutputCategoryRequest request)
        {
            if (request.Id == 0)
            {
                var OutputCategory = request.MapTo<KeyOutputCategory>();
                DataContext.KeyOutputCategories.Add(OutputCategory);
            }
            else
            {
                var OutputCategory = DataContext.KeyOutputCategories.FirstOrDefault(x => x.Id == request.Id);
                if (OutputCategory != null)
                {
                    request.MapPropertiesToInstance<KeyOutputCategory>(OutputCategory);
                }

            }
            DataContext.SaveChanges();
            return new SaveOutputCategoryRespone
            {
                IsSuccess = true,
                Message = "Output Category has been saved"
            };
        }


        public GetOutputCategoryResponse GetOutputCategory(GetOutputCategoryRequest request)
        {
            return DataContext.KeyOutputCategories.FirstOrDefault(x => x.Id == request.Id).MapTo<GetOutputCategoryResponse>();
        }


        public DeleteOutputCategoryResponse DeleteOutputCategory(DeleteOutputCategoryRequest request)
        {
            try
            {
                var OutputCategory = new KeyOutputCategory { Id = request.Id };
                DataContext.KeyOutputCategories.Attach(OutputCategory);
                DataContext.KeyOutputCategories.Remove(OutputCategory);
                DataContext.SaveChanges();

                return new DeleteOutputCategoryResponse
                {
                    IsSuccess = true,
                    Message = "The Output Category has been deleted successfully"
                };
            }
            catch(DbUpdateException exception)
            {
                return new DeleteOutputCategoryResponse
                {
                    IsSuccess = false,
                    Message = exception.Message
                };
            }
        }


        public IEnumerable<KeyOutputCategory> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOutputCategories.AsQueryable();
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
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
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


        public GetActiveOutputCategoriesResponse GetActiveOutputCategories(bool withDeepRelations = true)
        {
            var query = DataContext.KeyOutputCategories
                 .Include(x => x.KeyOutputs);

            if(withDeepRelations){
                query = query.Include(x => x.KeyOutputs.Select(y => y.Measurement))
                 .Include(x => x.KeyOutputs.Select(y => y.Kpis))
                 .Include(x => x.KeyOutputs.Select(y => y.KeyAssumptions));
            }

            return new GetActiveOutputCategoriesResponse
            {

                OutputCategories = query.Where(x => x.IsActive == true && x.KeyOutputs.Any(y => y.IsActive))
                .MapTo<GetActiveOutputCategoriesResponse.OutputCategoryResponse>()
            };
        }
    }
}
