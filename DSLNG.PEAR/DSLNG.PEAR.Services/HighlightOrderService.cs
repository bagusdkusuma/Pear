

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using DSLNG.PEAR.Services.Responses.HighlightOrder;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class HighlightOrderService : BaseService, IHighlightOrderService
    {
        public HighlightOrderService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public GetHighlightOrdersResponse GetHighlights(GetHighlightOrdersRequest request)
        {

            int totalRecords;
            var query = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                query = query.Skip(request.Skip).Take(request.Take);
            }
            var highlights = query.ToList();

            var response = new GetHighlightOrdersResponse();
            response.HighlightOrders = highlights.MapTo<GetHighlightOrdersResponse.HighlightOrderResponse>();
            var staticHighlights = DataContext.StaticHighlightPrivileges.Include(x => x.RoleGroups).ToList();
            foreach(var staticHighlight in staticHighlights){
                var resp = new GetHighlightOrdersResponse.HighlightOrderResponse();
                resp.Id = 1000 + staticHighlight.Id;
                resp.Text = staticHighlight.Name;
                resp.RoleGroupIds = staticHighlight.RoleGroups.Select(x => x.Id).ToArray();
                resp.Value = "static";
                response.HighlightOrders.Insert(0, resp);
            }
            response.TotalRecords = totalRecords;
            response.TotalRecords += staticHighlights.Count;

            return response;
        }

        private IEnumerable<SelectOption> SortData(string search, IDictionary<string, System.Data.SqlClient.SortOrder> sortingDictionary, out int totalRecords)
        {
            var exception = new string[] { "alert" };
            var data = DataContext.SelectOptions.Include(x => x.Group)
                .Include(x => x.RoleGroups)
                .Where(x => x.Select.Name == "highlight-types" && !exception.Contains(x.Value));
                //.Where(x => x.Select.Name == "highlight-types" && !exception.Contains(x.Value)).Where(x => x.IsDashboard == true);
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Text.Contains(search) || x.Value.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Text":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Text)
                                   : data.OrderByDescending(x => x.Text);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Order);
                        break;
                    case "GroupId":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Group.Id).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Group.Id).ThenBy(x => x.Order);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }

        public SaveHighlightOrderResponse SaveHighlight(SaveHighlightOrderRequest request)
        {
            try
            {
                var selectOption = DataContext.SelectOptions.Include(x => x.RoleGroups).First(x => x.Id == request.Id);
                //DataContext.SelectOptions.Attach(selectOption);
                if (request.Order.HasValue)
                {
                    selectOption.Order = request.Order.Value;
                }
                if (request.IsActive.HasValue)
                {
                    selectOption.IsActive = request.IsActive.Value;
                }
                if (request.GroupId != 0) {
                    var group = new HighlightGroup { Id = request.GroupId };
                    DataContext.HighlightGroups.Attach(group);
                    selectOption.Group = group;
                }
                if(request.RoleGroupIds.Count() > 0){
                    selectOption.RoleGroups = DataContext.RoleGroups.Where(x => request.RoleGroupIds.Contains(x.Id)).ToList();
                }
                DataContext.SaveChanges();
                return new SaveHighlightOrderResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully save highlight order"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveHighlightOrderResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }


        public SaveStaticHighlightOrderResponse SaveStaticHighlight(SaveStaticHighlightOrderRequest request)
        {
            try
            {
                var staticHighlight = DataContext
                    .StaticHighlightPrivileges
                    .Include(x => x.RoleGroups)
                    .First(x => x.Id == request.Id);
                if (request.RoleGroupIds.Count() > 0) {
                    staticHighlight.RoleGroups = DataContext
                        .RoleGroups
                        .Where(x => request.RoleGroupIds.Contains(x.Id))
                        .ToList();
                }
                DataContext.SaveChanges();
                return new SaveStaticHighlightOrderResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully save highlight order"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveStaticHighlightOrderResponse
                {
                    IsSuccess = false,
                    Message = "An error occure, please contact the administrator for further information"
                };
            }
        }


        public GetStaticHighlightOrdersResponse GetStaticHighlights(GetStaticHighlightOrdersRequest request)
        {
            var query = DataContext.StaticHighlightPrivileges.Include(x => x.RoleGroups).AsQueryable();
            return new GetStaticHighlightOrdersResponse
            {
                HighlightOrders = request.Take == -1? query.ToList().MapTo<GetStaticHighlightOrdersResponse.HighlightOrderResponse>() : query.Skip(request.Skip).Take(request.Take).ToList().MapTo<GetStaticHighlightOrdersResponse.HighlightOrderResponse>(),
                TotalRecords = query.Count()
            };
        }
    }
}
