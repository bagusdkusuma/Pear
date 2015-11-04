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

namespace DSLNG.PEAR.Services
{
    public class OperationalDataService : BaseService, IOperationalDataService
    {
        public OperationalDataService(IDataContext context) : base(context) {}




        public GetOperationalDatasResponse GetOperationalDatas(GetOperationalDatasRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetOperationalDatasResponse { Count = DataContext.KeyOperasionalDatas.Count() };
            }
            else
            {
                return new GetOperationalDatasResponse
                {
                    OperationalDatas = DataContext.KeyOperasionalDatas.OrderByDescending(x => x.Id)
                    .Include(x => x.KeyOperation).Include(x => x.Kpi)
                    .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
                };
            }
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
            if (request.Id == 0)
            {
                var OperationalData = request.MapTo<OperationDataConfiguration>();
                OperationalData.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
                OperationalData.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
                DataContext.KeyOperasionalDatas.Add(OperationalData);

            }
            else
            {
                var OperationalData = DataContext.KeyOperasionalDatas.FirstOrDefault(x => x.Id == request.Id);
                if (OperationalData != null)
                {
                    var operational = request.MapPropertiesToInstance<OperationDataConfiguration>(OperationalData);
                    operational.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
                    operational.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
                }
            }
            DataContext.SaveChanges();
            return new SaveOperationalDataResponse
            {
                IsSuccess = true,
                Message = "Operational Data has been Save"
            };
        }


        public GetOperationalDataResponse GetOperationalData(GetOperationalDataRequest request)
        {
            return DataContext.KeyOperasionalDatas
                .Include(x => x.KeyOperation).Include(x => x.Kpi)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetOperationalDataResponse>();
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
    }
}
