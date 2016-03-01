using DSLNG.PEAR.Services.Requests.OutputCategory;
using DSLNG.PEAR.Services.Responses.OutputCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOutputCategoryService
    {
        GetOutputCategoriesResponse GetOutputCategories(GetOutputCategoriesRequest request);
        SaveOutputCategoryRespone SaveOutputCategory(SaveOutputCategoryRequest request);
        GetOutputCategoryResponse GetOutputCategory(GetOutputCategoryRequest request);
        DeleteOutputCategoryResponse DeleteOutputCategory(DeleteOutputCategoryRequest request);
        GetActiveOutputCategoriesResponse GetActiveOutputCategories(bool withDeepRelations = true);
    }
}
