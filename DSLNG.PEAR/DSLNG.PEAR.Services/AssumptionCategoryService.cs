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

namespace DSLNG.PEAR.Services
{
    public class AssumptionCategoryService : BaseService, IAssumptionCategoryService
    {
        public AssumptionCategoryService(IDataContext context) : base(context) { }


        public GetAssumptionCategoriesResponse GetAssumptionCategories(GetAssumptionCategoriesRequest request)
        {
            if (request.OnlyCount)
            {

                return new GetAssumptionCategoriesResponse { Count = DataContext.KeyAssumptionCategories.Count() };

            }
            else
            {
                return new GetAssumptionCategoriesResponse
                {
                    AssumptionCategorys = DataContext.KeyAssumptionCategories.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetAssumptionCategoriesResponse.AssumptionCategory>()

                };
            }
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
            var AssumptionCategory = new KeyAssumptionCategory { Id = request.Id };
            DataContext.KeyAssumptionCategories.Attach(AssumptionCategory);
            DataContext.KeyAssumptionCategories.Remove(AssumptionCategory);
            DataContext.SaveChanges();

            return new DeleteAssumptionCategoryResponse
            {
                IsSuccess = true,
                Message = "The Assumption Category has been deleted successfully"
            };
        }
    }
}
