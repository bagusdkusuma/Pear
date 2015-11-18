using System.Data.SqlClient;
using AutoMapper;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Menu;
using DSLNG.PEAR.Services.Responses.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class MenuService : BaseService, IMenuService
    {
        public MenuService(IDataContext dataContext)
            : base(dataContext)
        {

        }

        public GetSiteMenusResponse GetSiteMenus(GetSiteMenusRequest request)
        {
            var response = new GetSiteMenusResponse();
            var menus = new List<Data.Entities.Menu>();

            if (request.ParentId != null)
            {
                menus = DataContext.Menus.Where(x => x.IsActive == true && x.ParentId == request.ParentId && x.RoleGroups.Select(y => y.Id).Contains(request.RoleId)).OrderBy(x => x.Order).ToList();
            }
            else
            {
                menus = DataContext.Menus.Where(x => x.IsActive == true && x.ParentId == null && x.RoleGroups.Select(y => y.Id).Contains(request.RoleId) || x.ParentId == 0 && x.RoleGroups.Select(y => y.Id).Contains(request.RoleId)).OrderBy(x => x.Order).ToList();
            }

            if (request.IncludeChildren)
            {
                var logout = new Data.Entities.Menu
                {
                    Name = "Logout",
                    IsActive = true,
                    Url = "/Account/Logoff",
                    Parent = null,
                    Icon = "logout.png"
                };
                menus.Add(logout);

                //looping to get the children, we dont use Include because only include 1st level child menus
                foreach (var menu in menus)
                {
                    menu.Menus = this._GetMenuChildren(menu.Id, request.RoleId);
                    //if (menu.Name == "Setting" && menu.IsRoot == true) {
                    //    menu.Menus.Add(logout);
                    //}
                }
            }


            response.Menus = menus.MapTo<GetSiteMenusResponse.Menu>();
            ////set root menu active / selected
            ////set root menu active / selected
            //if (request.MenuId == null || request.MenuId == 0)
            //{
            //    response.MenuIdActive = DataContext.Menus.Where(x => x.ParentId == null || x.ParentId == 0).Select(x => x.Id).First();
            //}
            //else
            //{

            //}

            return response;
        }

        private ICollection<Data.Entities.Menu> _GetMenuChildren(int ParentId, int RoleId)
        {
            var Menus = new List<Data.Entities.Menu>();

            Menus = DataContext.Menus.Where(x => x.IsActive == true && x.ParentId == ParentId && x.RoleGroups.Select(y => y.Id).Contains(RoleId)).OrderBy(x => x.Order).ToList();

            if (Menus != null)
            {
                foreach (var menu in Menus)
                {
                    menu.Menus = this._GetMenuChildren(menu.Id, RoleId);
                }
            }


            return Menus;
        }

        public GetSiteMenuActiveResponse GetSiteMenuActive(GetSiteMenuActiveRequest request)
        {
            var response = new GetSiteMenuActiveResponse();
            //get the menu from url request
            var url_request = new StringBuilder("/").Append(request.Controller).ToString();
            if (!request.Action.ToLower().Equals("index"))
            {
                url_request = new StringBuilder(url_request).Append("/").Append(request.Action).ToString();
            }
            var url_controller = new StringBuilder("/").Append(request.Controller).ToString();

            try
            {
                //var menu = DataContext.Menus.Where(x => x.Url == request.Url).First();
                //var menu = DataContext.Menus.Where(x => x.Url.ToLower() == url_request).First();
                var menus = DataContext.Menus.Where(x => x.Url == url_request || x.Url.Contains(url_controller) || x.Url.Contains(url_request)).OrderBy(y => y.Id).ToList();
                url_request = this._CleanUpMenuUrl(url_request);
                if (menus.Count == 1)
                {
                    var menu = menus[0];
                    var RootMenu = this._GetActiveMenu(menu);
                    response = RootMenu.MapTo<GetSiteMenuActiveResponse>();
                    response.SelectedMenu = menu.MapTo<Data.Entities.Menu>();
                    response.IsSuccess = true;
                }
                else if (menus.Count > 1)
                {
                    int i = 0;
                    foreach (var menu in menus)
                    {
                        // skip inactive menu
                        //if (menu.IsActive == false) break;
                        string edited_menu_url = this._CleanUpMenuUrl(menu.Url.ToString());
                        if (menu.Url.ToLower() == request.Url.ToLower() || edited_menu_url.Equals(request.Url.ToLower()))
                        {
                            var root = this._GetActiveMenu(menu);
                            if (root.IsRoot)
                            {
                                response = root.MapTo<GetSiteMenuActiveResponse>();
                                response.SelectedMenu = menu.MapTo<Data.Entities.Menu>();
                                response.IsSuccess = true;
                            }
                            break;
                        }
                        else
                        {
                            i++;
                            if (edited_menu_url.Equals(url_request.ToLower()))
                            {
                                var root = this._GetActiveMenu(menu);
                                if (root.IsRoot)
                                {
                                    response = root.MapTo<GetSiteMenuActiveResponse>();
                                    response.SelectedMenu = menu.MapTo<Data.Entities.Menu>();
                                    response.IsSuccess = true;
                                }
                                break;
                            }
                            else
                            {
                                if (i == menus.Count)
                                {
                                    Data.Entities.Menu parent = null;
                                    Data.Entities.Menu root = null;
                                    if (menu.IsActive == false)
                                    {
                                        parent = menu.Parent;
                                        root = this._GetActiveMenu(parent);
                                        response = root.MapTo<GetSiteMenuActiveResponse>();
                                        response.SelectedMenu = parent.MapTo<Data.Entities.Menu>();
                                    }
                                    else {
                                        root = this._GetActiveMenu(menu);
                                        response = root.MapTo<GetSiteMenuActiveResponse>();
                                        response.SelectedMenu = menu.MapTo<Data.Entities.Menu>();
                                    }

                                    response.IsSuccess = true;
                                    break;

                                }
                            }
                        }

                    }
                }
                else
                {
                    var menu = DataContext.Menus.First(m => m.Id == 1);
                    menu = this._GetActiveMenu(menu);
                    response = menu.MapTo<GetSiteMenuActiveResponse>();
                    response.SelectedMenu = menu.MapTo<Data.Entities.Menu>();
                    response.IsSuccess = false;
                }
                //var menu = DataContext.Menus.Where(x => x.Url.ToLower() == url_request || x.Url.ToLower().Contains(url_request)).OrderBy(y=>y.Id).First();
                //menu = this._GetActiveMenu(menu);
                //response = menu.MapTo<GetSiteMenuActiveResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                var menu = DataContext.Menus.First(m => m.Id == 1);
                response = menu.MapTo<GetSiteMenuActiveResponse>();

                response.Message = x.Message;
                return response;
            }
        }

        private Data.Entities.Menu _GetActiveMenu(Data.Entities.Menu menu)
        {
            if (!menu.IsRoot)
            {
                menu = DataContext.Menus.Where(x => x.Id == menu.ParentId).First();
                return this._GetActiveMenu(menu);
            }
            return menu;
        }

        public GetMenusResponse GetMenus(GetMenusRequest request)
        {
            IQueryable<Data.Entities.Menu> menus;
            if (request.Take != 0)
            {
                menus = DataContext.Menus.OrderBy(x => x.Order).Skip(request.Skip).Take(request.Take);
            }
            else
            {
                menus = DataContext.Menus;
            }

            var response = new GetMenusResponse() { Menus = menus.MapTo<GetMenusResponse.Menu>() };

            return response;
        }

        public GetMenusResponse GetMenusForGrid(GetMenusRequest request)
        {
            int totalRecords;
            var menus = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                menus = menus.Skip(request.Skip).Take(request.Take);
            }

            var response = new GetMenusResponse();
            response.Menus = menus.ToList().MapTo<GetMenusResponse.Menu>();
            response.TotalRecords = totalRecords;

            return response;
        }

        public GetMenuResponse GetMenu(GetMenuRequest request)
        {
            try
            {
                var menu = DataContext.Menus.Include(x => x.RoleGroups).First(x => x.Id == request.Id);
                var response = menu.MapTo<GetMenuResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetMenuResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public CreateMenuResponse Create(CreateMenuRequest request)
        {
            var response = new CreateMenuResponse();
            try
            {
                var menu = request.MapTo<Data.Entities.Menu>();
                //set IsRoot if no menu as parent
                menu.IsRoot = request.ParentId <= 0;
                menu.ParentId = menu.IsRoot ? null : menu.ParentId;

                //check if has role group
                if (request.RoleGroupIds.Count > 0)
                {
                    menu.RoleGroups = new HashSet<Data.Entities.RoleGroup>();

                    foreach (int roleGroupId in request.RoleGroupIds)
                    {
                        var roleGroup = DataContext.RoleGroups.Where(r => r.Id == roleGroupId).First();

                        //add selected role group to menu
                        menu.RoleGroups.Add(roleGroup);
                    }
                }
                else
                {
                    menu.RoleGroups = null;
                }

                //ensure url end with slash
                menu.Url = menu.Url != null && menu.Url.Length > 0 ? _CleanUpMenuUrl(menu.Url) : menu.Url;

                DataContext.Menus.Add(menu);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Menu item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateMenuResponse Update(UpdateMenuRequest request)
        {
            var response = new UpdateMenuResponse();
            try
            {
                var menu = request.MapTo<Data.Entities.Menu>();
                var item = DataContext.Entry(menu);

                item.State = EntityState.Modified;
                //Load RoleGroups Collection
                item.Collection("RoleGroups").Load();

                //set IsRoot if no menu as parent
                menu.IsRoot = request.ParentId <= 0;
                menu.ParentId = menu.IsRoot ? null : menu.ParentId;
                //clear RoleGroups collection first
                menu.RoleGroups.Clear();

                if (request.RoleGroupIds.Count > 0)
                {
                    //menu.RoleGroups = new HashSet<Data.Entities.RoleGroup>();

                    foreach (int roleGroupId in request.RoleGroupIds)
                    {
                        var roleGroup = DataContext.RoleGroups.Find(roleGroupId);

                        //add selected role group to menu
                        menu.RoleGroups.Add(roleGroup);
                    }
                }

                //ensure url end with slash
                menu.Url = menu.Url != null && menu.Url.Length > 0 ? _CleanUpMenuUrl(menu.Url) : menu.Url;

                //DataContext.Menus.Attach(menu);
                //DataContext.Entry(menu).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Menu item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeleteMenuResponse Delete(int id)
        {
            var response = new DeleteMenuResponse();
            response.Id = id;
            try
            {
                var menu = new Data.Entities.Menu { Id = id };
                DataContext.Menus.Attach(menu);
                DataContext.Entry(menu).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Menu item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }


        public GetMenuResponse GetMenuByUrl(GetMenuRequestByUrl request)
        {
            try
            {
                //var role = DataContext.RoleGroups.First(x => x.Id == request.RoleId);
                //var menu = DataContext.Menus.Include(x => x.RoleGroups).Where(x=>x.RoleGroups == role).First(x => x.Url == request.Url);
                //var url = request.Url != null ? request.Url.Split('/') : null;
                //string authorized = "/";
                //if (url[1].Length > 0)
                //{
                //    authorized = string.Format("/{0}/", url[1]);
                //    //authorized += url[1];
                //}
                //var menu = DataContext.Menus.Include(x => x.RoleGroups).First(x => x.RoleGroups.Select(y => y.Id).Contains(request.RoleId) && x.Url.Contains(authorized));
                var menu = DataContext.Menus.Include(x => x.RoleGroups).First(x => x.RoleGroups.Select(y=>y.Id).Contains(request.RoleId) && x.Url.Contains(request.Url));

                var response = menu.MapTo<GetMenuResponse>();
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetMenuResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        private String _CleanUpMenuUrl(String Url)
        {

            if (Url.IndexOf('?') >= 0)
            {
                return Url;
            }
            else
            {
                var newUrl = Url.TrimEnd(new Char[] { '/' }) + '/';
                newUrl = '/' + newUrl.TrimStart(new Char[] { '/' });

                return newUrl;
            }
        }


        public GetMenuResponse GetParentMenu(GetParentMenuRequest request)
        {
            try
            {
                var menu = DataContext.Menus.Include(x => x.RoleGroups).First(x => x.ParentId == request.ParentId);
                var response = menu.MapTo<GetMenuResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetMenuResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }


        public GetRootMenuResponse GetRootMenu(GetRootMenuRequest request)
        {
            var menu = DataContext.Menus.Include(x => x.Parent)
                .Include(x => x.Parent.Parent)
                .Include(x => x.Parent.Parent.Parent)
                .Include(x => x.Parent.Parent.Parent.Parent).FirstOrDefault(x => x.Url == request.AbsolutePath);
            var IsNotRoot = true;
            var i = 0;
            while (menu != null && IsNotRoot && i < 10) {
                if (menu.IsRoot)
                {
                    IsNotRoot = false;
                    return new GetRootMenuResponse { RootName = menu.Name };
                }
                else {
                    menu = menu.Parent;
                }
                i++;
            }
            return new GetRootMenuResponse();
        }

        private IEnumerable<Data.Entities.Menu> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Menus.Include(x=> x.RoleGroups).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Module.Contains(search) || x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Module":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Module).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Module).ThenBy(x => x.Order);
                        break;
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                        break;
                    case "IsRoot":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.IsRoot).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.IsRoot).ThenBy(x => x.Order);
                        break;
                    default:
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Order);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }
    }
}
