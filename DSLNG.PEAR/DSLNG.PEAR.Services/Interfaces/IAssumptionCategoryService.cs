using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using DSLNG.PEAR.Services.Responses.AssumptionCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IAssumptionCategoryService
    {
        GetAssumptionCategoriesResponse GetAssumptionCategories(GetAssumptionCategoriesRequest request);
        SaveAssumptionCategoryResponse SaveAssumptionCategory(SaveAssumptionCategoryRequest request);
        GetAssumptionCategoryResponse GetAssumptionCategory(GetAssumptionCategoryRequest request);
        DeleteAssumptionCategoryResponse DeleteAssumptionCategory(DeleteAssumptionCategoryRequest request);
    }
}
