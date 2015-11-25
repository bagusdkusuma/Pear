using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Services.Responses.OperationalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class OperationalDataService : BaseService, IOperationalDataService
    {
        public OperationalDataService(IDataContext context) : base(context) {}




        public GetOperationalDatasResponse GetOperationalDatas(GetOperationalDatasRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOperationalDatasResponse
            {
                TotalRecords = totalRecords,
                OperationalDatas = data.ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetOperationalDatasResponse { Count = DataContext.KeyOperasionalDatas.Count() };
            //}
            //else
            //{
            //    return new GetOperationalDatasResponse
            //    {
            //        OperationalDatas = DataContext.KeyOperasionalDatas.OrderByDescending(x => x.Id)
            //        .Include(x => x.KeyOperation).Include(x => x.Kpi)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
            //    };
            //}
        }


        public GetOperationalSelectListResponse GetOperationalSelectList()
        {
            return new GetOperationalSelectListResponse
            {
                Operations = DataContext.KeyOperations.ToList().MapTo<GetOperationalSelectListResponse.Operation>(),
                KPIS = DataContext.Kpis.ToList().MapTo<GetOperationalSelectListResponse.KPI>()
            };
        }


        public SaveOperationalDataResponse SaveOperationalData(SaveOperationalDataRequest request)
        {
            //if (request.Id == 0)
            //{
            //    var OperationalData = request.MapTo<KeyOperationData>();
            //    OperationalData.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
            //    OperationalData.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
            //    DataContext.KeyOperasionalDatas.Add(OperationalData);

            //}
            //else
            //{
            //    var OperationalData = DataContext.KeyOperasionalDatas.FirstOrDefault(x => x.Id == request.Id);
            //    if (OperationalData != null)
            //    {
            //        var operational = request.MapPropertiesToInstance<KeyOperationData>(OperationalData);
            //        operational.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
            //        operational.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
            //    }
            //}
            //DataContext.SaveChanges();
            //return new SaveOperationalDataResponse
            //{
            //    IsSuccess = true,
            //    Message = "Operational Data has been Save"
            //};
            throw new NotImplementedException();
        }


        public GetOperationalDataResponse GetOperationalData(GetOperationalDataRequest request)
        {
            //return DataContext.KeyOperasionalDatas
            //    .Include(x => x.KeyOperation).Include(x => x.Kpi)
            //    .FirstOrDefault(x => x.Id == request.Id).MapTo<GetOperationalDataResponse>();
            throw new NotImplementedException();
        }


        public DeleteOperationalDataResponse DeleteOperationalData(DeleteOperationalDataRequest request)
        {
            var checkId = DataContext.KeyOperasionalDatas.FirstOrDefault(x => x.Id == request.Id);
            if (checkId != null)
            {
                DataContext.KeyOperasionalDatas.Attach(checkId);
                DataContext.KeyOperasionalDatas.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteOperationalDataResponse
            {
                IsSuccess = true,
                Message = "Operational Data has been deleted successfully"
            };
        }

        public IEnumerable<KeyOperationData> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOperasionalDatas.Include(x => x.KeyOperation).Include(x => x.Kpi)
                .Include(x => x.KeyOperation.Kpi).Include(x => x.Scenario).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Kpi.Name.Contains(search) || x.Scenario.Name.Contains(search) || x.KeyOperation.Kpi.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Scenario":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Scenario.Name)
                            : data.OrderByDescending(x => x.Scenario.Name);
                        break;
                    case "KeyOperation":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.KeyOperation.Kpi.Name)
                            : data.OrderByDescending(x => x.KeyOperation.Kpi.Name);
                        break;
                    case "Kpi":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Kpi.Name)
                            : data.OrderByDescending(x => x.Kpi.Name);
                        break;
                    case "Value":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Value)
                            : data.OrderByDescending(x => x.Value);
                        break;
                    case "Periode":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Periode)
                            : data.OrderByDescending(x => x.Periode);
                        break;
                    case "PeriodeType":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.PeriodeType)
                            : data.OrderByDescending(x => x.PeriodeType);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
            //throw new NotImplementedException();


        }
    }
}
