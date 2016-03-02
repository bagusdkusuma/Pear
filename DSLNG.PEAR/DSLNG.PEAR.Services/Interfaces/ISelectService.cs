using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Responses.Select;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface ISelectService
    {
        CreateSelectResponse Create(CreateSelectRequest request);
        UpdateSelectResponse Update(UpdateSelectRequest request);
        DeleteSelectResponse Delete(int id);
        GetSelectResponse GetSelect(GetSelectRequest request);
        GetSelectsResponse GetSelects(GetSelectsRequest request);
        GetSelectsResponse GetSelectsForGrid(GetSelectsRequest request);
        IList<Dropdown> GetHighlightTypesDropdown();
    }
}
