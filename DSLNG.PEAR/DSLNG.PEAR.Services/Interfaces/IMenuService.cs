using DSLNG.PEAR.Services.Requests.Menu;
using DSLNG.PEAR.Services.Responses.Menu;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IMenuService
    {
        GetSiteMenusResponse GetSiteMenus(GetSiteMenusRequest request);
        GetSiteMenuActiveResponse GetSiteMenuActive(GetSiteMenuActiveRequest request);
        GetMenuResponse GetMenu(GetMenuRequest request);
        GetMenuResponse GetParentMenu(GetParentMenuRequest request);
        GetMenuResponse GetMenuByUrl(GetMenuRequestByUrl request);
        GetMenusResponse GetMenus(GetMenusRequest request);
        GetMenusResponse GetMenusForGrid(GetMenusRequest request);
        CreateMenuResponse Create(CreateMenuRequest request);
        UpdateMenuResponse Update(UpdateMenuRequest request);
        DeleteMenuResponse Delete(int id);
        GetRootMenuResponse GetRootMenu(GetRootMenuRequest request);
        GetMenuPrivilegeResponse GetMenuPrivilege(GetMenuPrivilegeRequest request);
    }
}
