using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Der;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IDerService
    {
        GetDersResponse GetDers(GetDersRequest request);
        GetDerResponse GetDerById(int id);
        CreateOrUpdateResponse CreateOrUpdate(CreateOrUpdateDerRequest request);
        GetActiveDerResponse GetActiveDer();
        GetDerItemResponse GetDerItem(GetDerItemRequest request);
        CreateOrUpdateDerLayoutResponse CreateOrUpdateDerLayout(CreateOrUpdateDerLayoutRequest request);
        GetDerLayoutsResponse GetDerLayouts();
        GetDerLayoutitemsResponse GetDerLayoutItems(int derLayoutId);
        GetDerLayoutitemResponse GetDerLayoutItem(int id);
        SaveLayoutItemResponse SaveLayoutItem(SaveLayoutItemRequest request);
        GetDerLayoutResponse GetDerLayout(int id);
        GetOriginalDataResponse GetOriginalData(int layoutId, DateTime date);
        SaveOriginalDataResponse SaveOriginalData(SaveOriginalDataRequest request);
        GetDafwcDataResponse GetDafwcData(int id, DateTime date);
        DeleteDerLayoutItemResponse DeleteLayoutItem(int id, string type);
        DeleteDerLayoutItemResponse DeleteLayoutItem(DeleteDerLayoutItemRequest request);
        BaseResponse DeleteLayout(int id);
        BaseResponse DeleteLayout(DeleteDerlayoutRequest request);
        GetKpiValueResponse GetKpiValue(GetKpiValueRequest request);
        BaseResponse DeleteFilename(string filename, DateTime date);
        BaseResponse DeleteFilename(DeleteFilenameRequest request);
        bool IsDerExisted(DateTime date, out int revision);
    }

    
}
