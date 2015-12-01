using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.AssumptionCategory;
using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class AssumptionCategoryService : BaseService, IAssumptionCategoryService
    {
        public AssumptionCategoryService(IDataContext context) : base(context) { }


        public GetAssumptionCategoriesResponse GetAssumptionCategories(GetAssumptionCategoriesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, request.IncludeAssumptionList, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetAssumptionCategoriesResponse
            {
                TotalRecords = totalRecords,
                AssumptionCategorys = data.ToList().MapTo<GetAssumptionCategoriesResponse.AssumptionCategory>()
            };

            //if (request.OnlyCount)
            //{

            //    return new GetAssumptionCategoriesResponse { Count = DataContext.KeyAssumptionCategories.Count() };

            //}
            //else
            //{
            //    return new GetAssumptionCategoriesResponse
            //    {
            //        AssumptionCategorys = DataContext.KeyAssumptionCategories.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionCategoriesResponse.AssumptionCategory>()

            //    };
            //}
        }


        public SaveAssumptionCategoryResponse SaveAssumptionCategory(SaveAssumptionCategoryRequest request)
        {

            if (request.Id == 0)
            {
                var AssumptionCategory = request.MapTo<KeyAssumptionCategory>();
                DataContext.KeyAssumptionCategories.Add(AssumptionCategory);
            }
            else
            {
                var AssumptionCategory = DataContext.KeyAssumptionCategories.FirstOrDefault(x => x.Id == request.Id);
                if (AssumptionCategory != null)
                {
                    request.MapPropertiesToInstance<KeyAssumptionCategory>(AssumptionCategory);
                }
            }
            DataContext.SaveChanges();
            return new SaveAssumptionCategoryResponse
            {
                IsSuccess = true,
                Message = "Assumption Category has been saved"
            };

        }


        public GetAssumptionCategoryResponse GetAssumptionCategory(GetAssumptionCategoryRequest request)
        {

            return DataContext.KeyAssumptionCategories.FirstOrDefault(x => x.Id == request.Id).MapTo<GetAssumptionCategoryResponse>();
        }



        public DeleteAssumptionCategoryResponse DeleteAssumptionCategory(DeleteAssumptionCategoryRequest request)
        {
            try
            {
                var assumptionCategory = new KeyAssumptionCategory { Id = request.Id };
                DataContext.KeyAssumptionCategories.Attach(assumptionCategory);
                DataContext.KeyAssumptionCategories.Remove(assumptionCategory);
                DataContext.SaveChanges();

                return new DeleteAssumptionCategoryResponse
                {
                    IsSuccess = true,
                    Message = "The Assumption Category has been deleted successfully"
                };
            }
            catch(DbUpdateException exception)
            {
                return new DeleteAssumptionCategoryResponse
                {
                    IsSuccess = false,
                    Message = exception.Message
                };
            }
        }


        public IEnumerable<KeyAssumptionCategory> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, bool includeAssumptionList, out int TotalRecords)
        {
            var data = DataContext.KeyAssumptionCategories.AsQueryable();
            if (includeAssumptionList) {
                data = data.Include(x => x.KeyAssumptions)
                    .Include(x => x.KeyAssumptions.Select(y => y.Measurement))
                    .Where(x => x.IsActive == true).OrderBy(x => x.Order);
            }
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name" :
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(X => X.Order)
                            : data.OrderByDescending(x => x.Name).ThenBy(X => X.Order);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Order)
                            : data.OrderByDescending(x => x.Order);
                        break;
                    case "IsActive" :
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive).ThenBy(X => X.Order)
                            : data.OrderByDescending(x => x.IsActive).ThenBy(X => X.Order);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }
    }
}
