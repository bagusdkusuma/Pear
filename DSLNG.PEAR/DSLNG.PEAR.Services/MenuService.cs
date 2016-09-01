using System.Collections;
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
using Menu = DSLNG.PEAR.Data.Entities.Menu;

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
            var requestUrl = request.Url.Split('/').Where(x => x != "").ToArray();
            string requestController = string.Empty;
            string requestAction = string.Empty;
            string requestId = string.Empty;
            if (requestUrl.Length > 0)
            {
                string ctrl = string.Empty;
                if (requestUrl[0].Contains("?"))
                {
                    var ctrls = requestUrl[0].Split('?');
                    if (ctrls.Length > 0)
                    {
                        ctrl = ctrls[0];
                    }
                }


                requestController = string.IsNullOrEmpty(ctrl) ? requestUrl[0] : ctrl;
            }

            if (requestUrl.Length > 1)
            {
                requestAction = requestUrl[1];
            }

            if (requestUrl.Length > 2)
            {
                requestId = requestUrl[2];
            }

            IDictionary<int, int> dictionary = new Dictionary<int, int>();

            var menus =
                DataContext.Menus
                .Include(x => x.Parent)
                .Where(x => x.Url.Contains(requestController)).ToList();

            foreach (var menu in menus)
            {
                var urlMenu = menu.Url.Split('/').Where(x => x != "").ToArray();
                string controller = string.Empty;
                string action = string.Empty;
                string id = string.Empty;
                int priority = 0;
                if (urlMenu.Length > 0)
                {
                    controller = urlMenu[0];
                }

                if (urlMenu.Length > 1)
                {
                    action = urlMenu[1];
                }

                if (urlMenu.Length > 2)
                {
                    id = urlMenu[2];
                }

                if (urlMenu.Length == requestUrl.Length)
                {
                    priority += 1;
                }

                if (controller == requestController)
                {
                    priority += 1;
                }

                if (action == requestAction)
                {
                    priority += 1;
                }

                if (id == requestId)
                {
                    priority += 1;
                }

                dictionary.Add(menu.Id, priority);
            }

            var newMenu = new Data.Entities.Menu();
            var sortedDictionary = from x in dictionary orderby x.Value descending select x;
            if (sortedDictionary.Any())
            {
                newMenu = menus.Single(x => x.Id == sortedDictionary.ElementAt(0).Key);
            }

            var rootMenu = GetRootMenu2(newMenu);
            var response = rootMenu.MapTo<GetSiteMenuActiveResponse>();
            response.SelectedMenu = newMenu.MapTo<Data.Entities.Menu>();
            var history = new List<int>();
            history.Add(newMenu.Id);
            response.HistoryMenu = GetHistoryMenu(newMenu, history);
            return response;
        }

        private List<int> GetHistoryMenu(Menu newMenu, List<int> history)
        {
            if (newMenu.Parent == null)
            {
                return history;
            }
            else
            {
                var parent = DataContext.Menus.First(x => x.Id == newMenu.ParentId);
                history.Add(parent.Id);
                return GetHistoryMenu(parent, history);
            }
        }

        private Data.Entities.Menu GetRootMenu2(Menu newMenu)
        {
            if (newMenu.ParentId == 0 || newMenu.ParentId == null)
            {
                return newMenu;
            }

            var menu = DataContext.Menus.First(x => x.Id == newMenu.ParentId);
            return GetRootMenu2(menu);
        }

        /*public GetSiteMenuActiveResponse GetSiteMenuActive(GetSiteMenuActiveRequest request)
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
         }*/

        private Data.Entities.Menu _GetActiveMenu(Data.Entities.Menu menu)
        {
            if (!menu.IsRoot)
            {
                if (menu.Parent.ParentId > 0)
                {
                    menu = DataContext.Menus.First(x => x.Id == menu.ParentId);
                    return this._GetActiveMenu(menu);
                }
            }

            return menu;
        }

        public GetMenusResponse GetMenus(GetMenusRequest request)
        {
            IQueryable<Data.Entities.Menu> menus;
            if (request.Take != 0)
            {
                menus = DataContext.Menus.AsNoTracking().OrderBy(x => x.Order).Skip(request.Skip).Take(request.Take);
            }
            else
            {
                menus = DataContext.Menus.AsNoTracking();
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
                var menu = DataContext.Menus.AsNoTracking().Include(x => x.RoleGroups).First(x => x.Id == request.Id);
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
                if (request.AddParent && !menu.IsRoot)
                {
                    AddParentMenu(menu.ParentId, request.RoleGroupIds);
                }
                response.IsSuccess = true;
                response.Message = "Menu item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        private void AddParentMenu(int? parentId, List<int> RoleGroupIds)
        {
            if (parentId.HasValue && parentId.Value > 0)
            {
                try
                {
                    var menu = DataContext.Menus.Find(parentId);
                    var item = DataContext.Entry(menu);
                    var state = item.State;
                    item.State = EntityState.Modified;
                    item.Collection("RoleGroups").Load();
                    var dicts = menu.RoleGroups.ToDictionary(x => x.Id);
                    if (RoleGroupIds.Count() > 0)
                    {
                        foreach (var role in RoleGroupIds)
                        {
                            dicts[role] = DataContext.RoleGroups.Find(role);
                        }
                        //merge existing with new group
                        var merged = dicts.Values.ToList();
                        //remove existing group
                        menu.RoleGroups.Clear();
                        foreach (var roleGroup in merged)
                        {
                            menu.RoleGroups.Add(roleGroup);
                        }
                        menu.Url = menu.Url != null && menu.Url.Length > 0 ? _CleanUpMenuUrl(menu.Url) : menu.Url;
                    }
                    DataContext.SaveChanges();
                    if (menu.ParentId.HasValue && menu.ParentId.Value > 0)
                    {
                        AddParentMenu(menu.ParentId.Value, RoleGroupIds);
                    }
                }
                catch (DbUpdateException ex)
                {

                    throw;
                }

            }

        }

        //private void AddParentMenu(UpdateMenuRequest request)
        //{
        //    Update(request);
        //    if (request.ParentId != null && request.ParentId > 0)
        //    {
        //        var parentMenu = GetMenu(new GetMenuRequest { Id = (int)request.ParentId }).MapTo<UpdateMenuRequest>();
        //        parentMenu.RoleGroupIds = request.RoleGroupIds;
        //        AddParentMenu(parentMenu);
        //    }
        //}

        #region new_update
        //public UpdateMenuResponse Update(UpdateMenuRequest request)
        //{
        //    var response = new UpdateMenuResponse();
        //    if (request.Id == 0) return response;
        //    try
        //    {
        //        var currentValue = DataContext.Menus.Find(request.Id);
        //        var menu = request.MapTo<Data.Entities.Menu>();
        //        var item = DataContext.Entry(currentValue);//.State = EntityState.Modified;
        //        item.State = EntityState.Modified;
        //        item.CurrentValues.SetValues(menu);
        //        //DataContext.Entry(currentValue).CurrentValues.SetValues(menu);

        //        //var item = DataContext.Entry(menu);
        //        //if (item.State == EntityState.Detached)
        //        //{
        //        //    DataContext.Menus.Attach(menu);
        //        //}
        //        //item.State = EntityState.Modified;
        //        ////Load RoleGroups Collection
        //        //item.Collection("RoleGroups").Load();

        //        ////set IsRoot if no menu as parent
        //        //menu.IsRoot = request.ParentId <= 0;
        //        //menu.ParentId = menu.IsRoot ? null : menu.ParentId;
        //        ////clear RoleGroups collection first
        //        //menu.RoleGroups.Clear();
        //        item.Collection("RoleGroups").Load();
        //        menu.IsRoot = request.ParentId <= 0;
        //        menu.ParentId = menu.IsRoot ? null : menu.ParentId;
        //        //currentValue.RoleGroups.Clear();

        //        var dicts = menu.RoleGroups.ToDictionary(x => x.Id);
        //        if (request.RoleGroupIds.Count > 0)
        //        {
        //            foreach (var role in request.RoleGroupIds)
        //            {
        //                dicts[role] = DataContext.RoleGroups.Find(role);
        //            }
        //            var merged = dicts.Values.ToList();
        //            menu.RoleGroups.Clear();

        //            foreach (var dict in merged)
        //            {
        //                menu.RoleGroups.Add(dict);
        //            }
        //        }
        //        //remove existing RoleGroup


        //        ///Removed due to performance issue
        //        //List<int> existing = new List<int>(menu.RoleGroups.Count());
        //        //foreach (var exis in menu.RoleGroups)
        //        //{
        //        //    existing.Add(exis.Id);
        //        //}
        //        //List<int> inter = existing.Union(request.RoleGroupIds).ToList();
        //        //if (inter.Count > 0)
        //        //{
        //        //    //menu.RoleGroups = new HashSet<Data.Entities.RoleGroup>();

        //        //    foreach (int roleGroupId in inter)
        //        //    {
        //        //        var roleGroup = DataContext.RoleGroups.Find(roleGroupId);

        //        //        //add selected role group to menu
        //        //        menu.RoleGroups.Add(roleGroup);
        //        //        //currentValue.RoleGroups.Add(roleGroup);
        //        //    }
        //        //}

        //        //ensure url end with slash
        //        menu.Url = menu.Url != null && menu.Url.Length > 0 ? _CleanUpMenuUrl(menu.Url) : menu.Url;
        //        //currentValue.Url = currentValue.Url != null && currentValue.Url.Length > 0 ? _CleanUpMenuUrl(currentValue.Url) : currentValue.Url;
        //        //DataContext.Menus.Attach(menu);
        //        //DataContext.Entry(menu).State = EntityState.Modified;

        //        DataContext.SaveChanges();
        //        response.IsSuccess = true;
        //        response.Message = "Menu item has been updated successfully";
        //    }
        //    catch (DbUpdateException dbUpdateException)
        //    {
        //        response.Message = dbUpdateException.Message;
        //    }

        //    return response;
        //}
        #endregion
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
                menu.IsRoot = request.ParentId <= 0 || request.ParentId == null;
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
                var response = new GetMenuResponse();
                var menu = DataContext.Menus.AsNoTracking().Include(x => x.RoleGroups).FirstOrDefault(x => x.RoleGroups.Select(y => y.Id).Contains(request.RoleId) && x.Url.Contains(request.Url));
                if (menu != null)
                {
                    response = menu.MapTo<GetMenuResponse>();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "No Menu Found";
                }
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
            var absoluteAlternative = "Nothing";
            var absoluteSplit = request.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int result;
            if (absoluteSplit.Length > 0 && int.TryParse(absoluteSplit[absoluteSplit.Length - 1], out result))
            {
                absoluteAlternative = request.AbsolutePath.Replace(result.ToString(), "$/");
            }
            if (!request.AbsolutePath.EndsWith("/"))
            {
                request.AbsolutePath += "/";
            }
            var menu = DataContext.Menus.Include(x => x.Parent)
                .Include(x => x.Parent.Parent)
                .Include(x => x.Parent.Parent.Parent)
                .Include(x => x.Parent.Parent.Parent.Parent)
                .FirstOrDefault(x => x.Url == request.AbsolutePath || x.Url == absoluteAlternative);
            var IsNotRoot = true;
            var i = 0;
            while (menu != null && IsNotRoot && i < 10)
            {
                if (menu.IsRoot)
                {
                    IsNotRoot = false;
                    return new GetRootMenuResponse { RootName = menu.Name };
                }
                else
                {
                    menu = menu.Parent;
                }
                i++;
            }
            return new GetRootMenuResponse();
        }

        private IEnumerable<Data.Entities.Menu> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Menus.AsNoTracking().Include(x => x.RoleGroups).AsQueryable();
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


        public GetMenuPrivilegeResponse GetMenuPrivilege(GetMenuPrivilegeRequest request)
        {
            var response = new GetMenuPrivilegeResponse();
            try
            {
                response = DataContext.MenuRolePrivileges.AsNoTracking().Include(x => x.RolePrivilege).Include(y => y.Menu).Where(z => z.Menu.Id == request.Menu_Id && z.RolePrivilege.Id == request.RolePrivilege_Id).FirstOrDefault().MapTo<GetMenuPrivilegeResponse>();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            //var test = DataContext.MenuRolePrivileges.Include(x => x.RolePrivilege).Include(y => y.Menu).Where(z=>z.Menu.Id==request.Menu_Id && z.RolePrivilege.Id==request.RolePrivilege_Id).FirstOrDefault();

            //response = test.MapTo<GetMenuPrivilegeResponse>();
            ////response = DataContext.MenuRolePrivileges.Include(x => x.Menu).Where(x => x.Menu_Id == request.Menu_Id && x.RolePrivilege_Id == request.RolePrivilege_Id).Include(x => x.RolePrivilege).FirstOrDefault().MapTo<GetMenuPrivilegeResponse>();
            return response;
        }
    }
}
