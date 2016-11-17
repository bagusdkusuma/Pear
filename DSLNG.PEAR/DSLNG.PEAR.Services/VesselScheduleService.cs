
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Responses.VesselSchedule;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Data.SqlClient;

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
            else if (request.allActiveList)
            {
                var query = DataContext.VesselSchedules
                    .Include(x => x.Buyer)
                    .Include(x => x.Vessel)
                    .Include(x => x.Vessel.Measurement)
                    .Select(x => new
                    {
                        id = x.Id,
                        NextLoadingSchedules = x.NextLoadingSchedules.OrderByDescending(y => y.CreatedAt).Take(1).ToList(),
                        Buyer = x.Buyer,
                        Vessel = x.Vessel,
                        ETA = x.ETA,
                        ETD = x.ETD,
                        Location = x.Location,
                        SalesType = x.SalesType,
                        Type = x.Type,
                        VesselType = x.Vessel.Type,
                        IsActive = x.IsActive,
                        Cargo = x.Cargo,
                        Measurement = x.Vessel.Measurement.Name,
                        Capacity = x.Vessel.Capacity
                    });
                if (request.RemarkDate.HasValue) {
                    query = DataContext.VesselSchedules
                    .Include(x => x.Buyer)
                    .Include(x => x.Vessel)
                    .Include(x => x.Vessel.Measurement)
                    .Select(x => new
                    {
                        id = x.Id,
                        NextLoadingSchedules = x.NextLoadingSchedules.Where(y => y.CreatedAt <= request.RemarkDate.Value).OrderByDescending(y => y.CreatedAt).Take(1).ToList(),
                        Buyer = x.Buyer,
                        Vessel = x.Vessel,
                        ETA = x.ETA,
                        ETD = x.ETD,
                        Location = x.Location,
                        SalesType = x.SalesType,
                        Type = x.Type,
                        VesselType = x.Vessel.Type,
                        IsActive = x.IsActive,
                        Cargo = x.Cargo,
                        Measurement = x.Vessel.Measurement.Name,
                        Capacity = x.Vessel.Capacity
                    });
                }
                query = query.Where(x => x.IsActive == true);
                if (request.OrderByETDDesc) {
                    query = query.OrderByDescending(x => x.ETD);
                }
                return new GetVesselSchedulesResponse
                {
                    VesselSchedules = query.Select(
                        x => new GetVesselSchedulesResponse.VesselScheduleResponse
                        {
                            id = x.id,
                            Remark = x.NextLoadingSchedules.Count == 1 ? x.NextLoadingSchedules.FirstOrDefault().Remark : null,
                            RemarkDate = x.NextLoadingSchedules.Count == 1 ? x.NextLoadingSchedules.FirstOrDefault().CreatedAt : (DateTime?)null,
                            Buyer = x.Buyer.Name,
                            Vessel = x.Vessel.Name,
                            ETA = x.ETA,
                            ETD = x.ETD,
                            Location = x.Location,
                            SalesType = x.SalesType,
                            Type = x.Type,
                            IsActive = x.IsActive,
                            Cargo = x.Cargo,
                            VesselType = x.VesselType,
                            Measurement = x.Measurement,
                            Capacity = x.Capacity
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
                var vesselSchedule = request.MapTo<VesselSchedule>();
                if (request.Id == 0)
                {
                   
                    var buyer = DataContext.Buyers.First(x => x.Id == request.BuyerId);// new Buyer { Id = request.BuyerId };
                    //DataContext.Buyers.Attach(buyer);
                    var vessel = DataContext.Vessels.First(x => x.Id == request.VesselId);// new Vessel { Id = request.VesselId };
                    //DataContext.Vessels.Attach(vessel);
                    vesselSchedule.Name = string.Format("{0}-{1}-{2}-{3}", vessel.Name, buyer.Name, vessel.Type, vesselSchedule.Cargo);
                    vesselSchedule.Buyer = buyer;
                    vesselSchedule.Vessel = vessel;
                    DataContext.VesselSchedules.Add(vesselSchedule);
                }
                else
                {
                    vesselSchedule = DataContext.VesselSchedules.FirstOrDefault(x => x.Id == request.Id);
                    if (vesselSchedule != null)
                    {
                        request.MapPropertiesToInstance<VesselSchedule>(vesselSchedule);
                        var buyer = DataContext.Buyers.First(x => x.Id == request.BuyerId);// new Buyer { Id = request.BuyerId };
                        //DataContext.Buyers.Attach(buyer);
                        var vessel = DataContext.Vessels.First(x => x.Id == request.VesselId);// new Vessel { Id = request.VesselId };
                        //DataContext.Vessels.Attach(vessel);
                        vesselSchedule.Name = string.Format("{0}-{1}-{2}-{3}", vessel.Name, buyer.Name, vessel.Type, vesselSchedule.Cargo);
                        vesselSchedule.Buyer = buyer;
                        vesselSchedule.Vessel = vessel;
                    }
                }
                DataContext.SaveChanges();
                var response = new SaveVesselScheduleResponse
                {
                    IsSuccess = true,
                    Message = "Vessel Schedule has been saved"
                };
                vesselSchedule = DataContext.VesselSchedules.Include(x => x.Buyer)
                    .Include(x => x.Vessel)
                    .Include(x => x.Vessel.Measurement)
                    .Single(x => x.Id == vesselSchedule.Id);
                vesselSchedule.MapPropertiesToInstance(response);
                if (request.DerTransactionDate.HasValue) {
                    var latestRemark = DataContext.NextLoadingSchedules.Where(x => x.VesselSchedule.Id == vesselSchedule.Id && x.CreatedAt <= request.DerTransactionDate)
                   .OrderByDescending(x => x.CreatedAt)
                   .FirstOrDefault();
                    if (latestRemark != null)
                    {
                        response.RemarkDate = latestRemark.CreatedAt.ToString("dd-MM-yyyy");
                        response.Remark = latestRemark.Remark;
                    }
                }
               
                return response;
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


        public DeleteVesselScheduleResponse Delete(DeleteVesselScheduleRequest request)
        {
            try
            {
                var vesselSchedule = new VesselSchedule { Id = request.Id };
                DataContext.VesselSchedules.Attach(vesselSchedule);
                DataContext.VesselSchedules.Remove(vesselSchedule);
                DataContext.SaveChanges();
                return new DeleteVesselScheduleResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully delete this item"
                };
            }
            catch (DbUpdateException e) {
                if (e.InnerException.InnerException.Message.Contains("dbo.NextLoadingSchedules"))
                {
                    return new DeleteVesselScheduleResponse
                    {
                        IsSuccess = false,
                        Message = "The vessel schedule is being used by next loading schedule"
                    };
                }
                return new DeleteVesselScheduleResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
            catch (InvalidOperationException e)
            {
                return new DeleteVesselScheduleResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }


        public IEnumerable<VesselSchedule> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.VesselSchedules.Include(x => x.Buyer).Include(x => x.Vessel).Include(x => x.Vessel.Measurement).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Vessel.Name.Contains(search)
                    || x.Buyer.Name.Contains(search)
                    || x.Location.Contains(search)
                    || x.SalesType.Contains(search)
                    || x.Type.Contains(search)
                    || x.Cargo.Contains(search)
                    //|| x.Vessel.Measurement.Name.Contains(search)
                    );
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Vessel":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Vessel.Name)
                            : data.OrderByDescending(x => x.Vessel.Name);
                        break;
                    case "ETA":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ETA)
                            : data.OrderByDescending(x => x.ETA);
                        break;
                    case "ETD":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ETD)
                            : data.OrderByDescending(x => x.ETD);
                        break;
                    case "Buyer":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Buyer.Name)
                            : data.OrderByDescending(x => x.Buyer.Name);
                        break;
                    case "Location":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Location)
                            : data.OrderByDescending(x => x.Location);
                        break;
                    case "SalesType":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.SalesType)
                            : data.OrderByDescending(x => x.SalesType);
                        break;
                    case "Type":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Type)
                            : data.OrderByDescending(x => x.Type);
                        break;
                    case "Cargo":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Cargo)
                            : data.OrderByDescending(x => x.Cargo);
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


        public GetVesselSchedulesResponse GetVesselSchedulesForGrid(GetVesselSchedulesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetVesselSchedulesResponse
            {
                TotalRecords = totalRecords,
                VesselSchedules = data.ToList().MapTo<GetVesselSchedulesResponse.VesselScheduleResponse>()
            };
        }
    }
}
