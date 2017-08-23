using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Responses.InputData;
using DSLNG.PEAR.Services.Requests.InputData;
using DSLNG.PEAR.Data.Entities.InputOriginalData;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Responses;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class InputDataService : BaseService, IInputDataService
    {
        public InputDataService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public BaseResponse Delete(DeleteInputDataRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<Data.Entities.BaseAction>();
                var input = DataContext.InputData
                    .Include(x => x.GroupInputDatas)
                    .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders))
                    .First(x => x.Id == request.Id);
                if (input.GroupInputDatas.Count > 0)
                {
                    foreach (var item in input.GroupInputDatas.ToList())
                    {
                        if (item.InputDataKpiAndOrders.Count > 0)
                        {
                            foreach (var kpi in item.InputDataKpiAndOrders.ToList())
                            {
                                DataContext.InputDataKpiAndOrder.Remove(kpi);
                            }
                            DataContext.SaveChanges(action);
                        }
                        DataContext.GroupInputData.Remove(item);
                        DataContext.SaveChanges(action);
                    }
                }
                //DataContext.InputData.Attach(input);
                DataContext.InputData.Remove(input);
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = string.Format("Input Data item {0} deleted Successfully!", input.Name);
            }
            catch (DbUpdateException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public BaseResponse Delete(int id)
        {
            var response = new BaseResponse();
            try
            {
                var input = DataContext.InputData
                    .Include(x=>x.GroupInputDatas)
                    .Include(x=>x.GroupInputDatas.Select(y=>y.InputDataKpiAndOrders))
                    .First(x => x.Id == id);
                if (input.GroupInputDatas.Count > 0)
                {
                    foreach (var item in input.GroupInputDatas.ToList())
                    {
                        if (item.InputDataKpiAndOrders.Count > 0)
                        {
                            foreach (var kpi in item.InputDataKpiAndOrders.ToList())
                            {
                                DataContext.InputDataKpiAndOrder.Remove(kpi);
                            }
                            DataContext.SaveChanges();
                        }
                        DataContext.GroupInputData.Remove(item);
                        DataContext.SaveChanges();
                    }
                }
                //DataContext.InputData.Attach(input);
                DataContext.InputData.Remove(input);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = string.Format("Input Data item {0} deleted Successfully!", input.Name);
            }
            catch (DbUpdateException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public GetInputDataResponse GetInputData(int id)
        {
            var response = new GetInputDataResponse();
            try
            {
                var inputData = DataContext.InputData
                    .Include(x => x.GroupInputDatas)
                    .Include(x => x.Accountability)
                    .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders))
                    .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders.Select(z => z.Kpi)))
                    .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders.Select(z => z.Kpi.Measurement)))                    
                    .Single(x => x.Id == id);

                response = inputData.MapTo<GetInputDataResponse>();

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetInputDatasResponse GetInputData(GetInputDatasRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            var response = new GetInputDatasResponse();
            response.TotalRecords = totalRecords;
            response.InputDatas = data.ToList().MapTo<GetInputDatasResponse.InputData>();

            return response;
        }

        public GetInputDatasResponse GetInputDatas()
        {
            var response = new GetInputDatasResponse();
            try
            {
                var inputData = DataContext.InputData
                    .Include(x => x.Accountability)
                    .Include(x => x.GroupInputDatas)                    
                    .ToList();

                response.InputDatas = inputData.MapTo<GetInputDatasResponse.InputData>();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;

            }

            return response;
        }

        public SaveOrUpdateInputDataResponse SaveOrUpdateInputData(SaveOrUpdateInputDataRequest request)
        {
            var response = new SaveOrUpdateInputDataResponse();
            try
            {
                var action = request.MapTo<Data.Entities.BaseAction>();
                if (request.Id > 0)
                {
                    var inputData = DataContext.InputData
                        .Include(x => x.GroupInputDatas)
                        .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders))
                        .Include(x => x.GroupInputDatas.Select(y => y.InputDataKpiAndOrders.Select(z => z.Kpi)))
                        .Single(x => x.Id == request.Id);
                    inputData.Name = request.Name;
                    inputData.Accountability = DataContext.RoleGroups.Single(x => x.Id == request.AccountabilityId);                    
                    inputData.UpdatedBy = DataContext.Users.Single(x => x.Id == request.UpdatedById);
                    inputData.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
                    inputData.LastInput = DateTime.Now;
                    foreach (var groupInputData in inputData.GroupInputDatas.ToList())
                    {
                        foreach(var inputDataKpiAndOrder in groupInputData.InputDataKpiAndOrders.ToList())
                        {
                            DataContext.InputDataKpiAndOrder.Remove(inputDataKpiAndOrder);
                        }
                        DataContext.GroupInputData.Remove(groupInputData);
                    }

                    var groupInputDatas = new List<GroupInputData>();
                    foreach (var item in request.GroupInputDatas.ToList())
                    {
                        var groupInputData = new GroupInputData();
                        groupInputData.Name = item.Name;
                        groupInputData.Order = item.Order;
                        var kpiAndOrders = new List<InputDataKpiAndOrder>();
                        foreach (var kpi in item.InputDataKpiAndOrders.ToList())
                        {
                            kpiAndOrders.Add(new InputDataKpiAndOrder { Kpi = DataContext.Kpis.Single(x => x.Id == kpi.KpiId), Order = kpi.Order });
                        }
                        groupInputData.InputDataKpiAndOrders = kpiAndOrders;
                        groupInputDatas.Add(groupInputData);
                    }

                    inputData.GroupInputDatas = groupInputDatas;
                    DataContext.Entry(inputData).State = EntityState.Modified;
                    DataContext.SaveChanges(action);

                    response.IsSuccess = true;
                }
                else
                {
                    var inputData = request.MapTo<InputData>();
                    inputData.Accountability = DataContext.RoleGroups.Single(x => x.Id == request.AccountabilityId);                    
                    inputData.UpdatedBy = DataContext.Users.Single(x => x.Id == request.UpdatedById);
                    inputData.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
                    inputData.LastInput = DateTime.Now;
                    var groupInputDatas = new List<GroupInputData>();
                    foreach (var item in request.GroupInputDatas)
                    {
                        var groupInputData = new GroupInputData();
                        groupInputData.Name = item.Name;
                        groupInputData.Order = item.Order;
                        var kpiAndOrders = new List<InputDataKpiAndOrder>();
                        foreach (var kpi in item.InputDataKpiAndOrders)
                        {
                            kpiAndOrders.Add(new InputDataKpiAndOrder { Kpi = DataContext.Kpis.Single(x => x.Id == kpi.KpiId), Order = kpi.Order });
                        }
                        groupInputData.InputDataKpiAndOrders = kpiAndOrders;
                        groupInputDatas.Add(groupInputData);
                    }

                    inputData.GroupInputDatas = groupInputDatas;
                    DataContext.InputData.Add(inputData);
                    DataContext.SaveChanges(action);
                    response.IsSuccess = true;
                }
            }
            catch(Exception exception)
            {                
                response.Message = exception.Message;
            }

            return response;
            

        }

        private IEnumerable<InputData> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.InputData
                .AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Name.Contains(search) ||
                                       x.Name.Contains(search));
            }

            if (sortingDictionary != null && sortingDictionary.Count > 0)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "Name":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Name)
                                       : data.OrderByDescending(x => x.Name);
                            break;
                        case "PeriodeType":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.PeriodeType)
                                       : data.OrderByDescending(x => x.PeriodeType);
                            break;
                        case "Accountability":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Accountability.Name)
                                       : data.OrderByDescending(x => x.Accountability.Name);
                            break;
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Id)
                                       : data.OrderByDescending(x => x.Id);
                            break;
                    }
                }
            }

            totalRecords = data.Count();
            return data;
        }
    }
}
