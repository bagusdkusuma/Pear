using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Requests.Method;
using DSLNG.PEAR.Services.Responses.Method;
using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Common.Extensions;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class MethodService : BaseService, IMethodService
    {
        public MethodService(IDataContext dataContext) : base(dataContext)
        {

        }



        public GetMethodResponse GetMethod(GetMethodRequest request)
        {
            try
            {
                var method = DataContext.Methods.First(x => x.Id == request.Id);
                var response = method.MapTo<GetMethodResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetMethodResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public GetMethodsResponse GetMethods(GetMethodsRequest requests)
        {
            int totalRecord;
            var data = SortData(requests.Search, requests.SortingDictionary, out totalRecord);
            if (requests.Take != -1)
            {
                data = data.Skip(requests.Skip).Take(requests.Take);
            }

            return new GetMethodsResponse
            {
                TotalRecords = totalRecord,
                Methods = data.ToList().MapTo<GetMethodsResponse.Method>()
            };
            //var methods = DataContext.Methods.ToList();
            //var response = new GetMethodsResponse();
            //response.Methods = methods.MapTo<GetMethodsResponse.Method>();

            //return response;
        }

        public CreateMethodResponse Create(CreateMethodRequest request)
        {
            var response = new CreateMethodResponse();
            try
            {
                var method = request.MapTo<Method>();
                DataContext.Methods.Add(method);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Method item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateMethodResponse Update(UpdateMethodRequest request)
        {
            var response = new UpdateMethodResponse();
            try
            {
                var method = request.MapTo<Method>();
                DataContext.Methods.Attach(method);
                DataContext.Entry(method).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Method item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeleteMethodResponse Delete(int id)
        {
            var response = new DeleteMethodResponse();
            try
            {
                var method = new Method { Id = id };
                DataContext.Methods.Attach(method);
                DataContext.Entry(method).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Method item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }



        public IEnumerable<Method> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Methods.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name)
                            : data.OrderByDescending(x => x.Name);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }
    }
}
