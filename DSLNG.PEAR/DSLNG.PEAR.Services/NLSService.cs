
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.NLS;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Common.Extensions;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class NLSService : BaseService, INLSService
    {
        public NLSService(IDataContext dataContext) : base(dataContext) { }


        public GetNLSResponse GetNLS(GetNLSRequest request)
        {
            return DataContext.NextLoadingSchedules
                .Include(x => x.VesselSchedule)
                .Include(x => x.VesselSchedule.Vessel)
                .Include(x => x.VesselSchedule.Buyer)
                .FirstOrDefault().MapTo<GetNLSResponse>();
        }

        public GetNLSListResponse GetNLSList(GetNLSListRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetNLSListResponse { Count = DataContext.NextLoadingSchedules.Count() };
            }
            else {
                var query = DataContext.NextLoadingSchedules.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term)) {
                    query = query.Where(x => x.VesselSchedule.Vessel.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetNLSListResponse
                {
                    NLSList = query.ToList().MapTo<GetNLSListResponse.NLSResponse>()
                };
            }
        }

        public SaveNLSResponse SaveNLS(SaveNLSRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var nls = request.MapTo<NextLoadingSchedule>();
                    var vesselSchedule = new VesselSchedule { Id = request.VesselScheduleId };
                    DataContext.VesselSchedules.Attach(vesselSchedule);
                    nls.VesselSchedule = vesselSchedule;
                    DataContext.NextLoadingSchedules.Add(nls);
                }
                else
                {
                    var nls = DataContext.NextLoadingSchedules.FirstOrDefault(x => x.Id == request.Id);
                    if (nls != null)
                    {
                        request.MapPropertiesToInstance<NextLoadingSchedule>(nls);
                        var vesselSchedule = new VesselSchedule { Id = request.VesselScheduleId };
                        DataContext.VesselSchedules.Attach(vesselSchedule);
                        nls.VesselSchedule = vesselSchedule;
                    }
                }
                DataContext.SaveChanges();
                return new SaveNLSResponse
                {
                    IsSuccess = true,
                    Message = "Next Loading Schedule has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveNLSResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}
