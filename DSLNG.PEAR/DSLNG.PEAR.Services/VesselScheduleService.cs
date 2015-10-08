
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Responses.VesselSchedule;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class VesselScheduleService : BaseService, IVesselScheduleService
    {
        public VesselScheduleService(IDataContext dataContext) : base(dataContext) { }
        public GetVesselScheduleResponse GetVesselSchedule(GetVesselScheduleRequest request)
        {
            return DataContext.VesselSchedules
                .Include(x => x.Vessel)
                .Include(x => x.Buyer)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetVesselScheduleResponse>(); 
        }

        public GetVesselSchedulesResponse GetVesselSchedules(GetVesselSchedulesRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetVesselSchedulesResponse { Count = DataContext.VesselSchedules.Count() };
            }
            else if (request.allActiveList) {
                var query = DataContext.VesselSchedules
                    .Include(x => x.Buyer)
                    .Include(x => x.Vessel)
                    .Select(x => new { 
                        NextLoadingSchedules = x.NextLoadingSchedules.OrderByDescending(y => y.CreatedAt).Take(1).ToList(),
                        Buyer = x.Buyer,
                        Vessel = x.Vessel,
                        ETA = x.ETA,
                        ETD = x.ETD,
                        Location = x.Location,
                        SalesType = x.SalesType,
                        Type = x.Type,
                        IsActive = x.IsActive
                    });
                return new GetVesselSchedulesResponse
                {
                    VesselSchedules = query.Where(x => x.IsActive == true).Select(
                        x => new GetVesselSchedulesResponse.VesselScheduleResponse
                        {
                            Remark = x.NextLoadingSchedules.Count == 1? x.NextLoadingSchedules.FirstOrDefault().Remark : null,
                            RemarkDate = x.NextLoadingSchedules.Count == 1 ? x.NextLoadingSchedules.FirstOrDefault().CreatedAt : (DateTime?)null,
                            Buyer = x.Buyer.Name,
                            Vessel = x.Vessel.Name,
                            ETA = x.ETA,
                            ETD = x.ETD,
                            Location = x.Location,
                            SalesType = x.SalesType,
                            Type = x.Type,
                            IsActive = x.IsActive
                        }
                    ).ToList()
                };
            }
            else
            {
                var query = DataContext.VesselSchedules.Include(x => x.Buyer)
                    .Include(x => x.Vessel);
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Vessel.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetVesselSchedulesResponse
                {
                    VesselSchedules = query.ToList().MapTo<GetVesselSchedulesResponse.VesselScheduleResponse>()
                };
            }
        }

        public SaveVesselScheduleResponse SaveVesselSchedule(SaveVesselScheduleRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var vesselSchedule = request.MapTo<VesselSchedule>();
                    var buyer = new Buyer { Id = request.BuyerId };
                    DataContext.Buyers.Attach(buyer);
                    var vessel = new Vessel { Id = request.VesselId };
                    DataContext.Vessels.Attach(vessel);
                    vesselSchedule.Buyer = buyer;
                    vesselSchedule.Vessel = vessel;
                    DataContext.VesselSchedules.Add(vesselSchedule);
                }
                else
                {
                    var vesselSchedule = DataContext.VesselSchedules.FirstOrDefault(x => x.Id == request.Id);
                    if (vesselSchedule != null)
                    {
                        request.MapPropertiesToInstance<VesselSchedule>(vesselSchedule);
                        var buyer = new Buyer { Id = request.BuyerId };
                        DataContext.Buyers.Attach(buyer);
                        var vessel = new Vessel { Id = request.VesselId };
                        DataContext.Vessels.Attach(vessel);
                        vesselSchedule.Buyer = buyer;
                        vesselSchedule.Vessel = vessel;
                    }
                }
                DataContext.SaveChanges();
                return new SaveVesselScheduleResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveVesselScheduleResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}
