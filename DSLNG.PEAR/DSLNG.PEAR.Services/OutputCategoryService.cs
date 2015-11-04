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

namespace DSLNG.PEAR.Services
{
    public class OutputCategoryService : BaseService, IOutputCategoryService
    {

        public OutputCategoryService(IDataContext context) : base(context) { }

        GetOutputCategoriesResponse IOutputCategoryService.GetOutputCategories(GetOutputCategoriesRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetOutputCategoriesResponse { Count = DataContext.KeyOutputCategories.Count() };
            }
            else
            {
                return new GetOutputCategoriesResponse
                {
                    OutputCategories = DataContext.KeyOutputCategories.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOutputCategoriesResponse.OutputCategory>()
                };
            }
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
    }
}
