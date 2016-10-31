using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Responses.Vessel;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class VesselService : BaseService,IVesselService
    {
        public VesselService(IDataContext dataContext) : base(dataContext) { }

        public GetVesselResponse GetVessel(GetVesselRequest request)
        {
            return DataContext.Vessels
                .Include(x => x.Measurement)
                .FirstOrDefault(x => x.Id == request.Id)
                .MapTo<GetVesselResponse>();
        }

        public GetVesselsResponse GetVessels(GetVesselsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetVesselsResponse { Count = DataContext.Vessels.Count() };
            }
            else
            {
                var query = DataContext.Vessels
                    .Include(x => x.Measurement);
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetVesselsResponse
                {
                    Vessels = query.ToList().MapTo<GetVesselsResponse.VesselResponse>()
                };
            }
        }

        public SaveVesselResponse SaveVessel(SaveVesselRequest request)
        {
            try
            {
                var vessel = request.MapTo<Vessel>();
                if (request.Id == 0)
                {
                    var measurement = new Measurement { Id = request.MeasurementId };
                    DataContext.Measurements.Attach(measurement);
                    vessel.Measurement = measurement;
                    DataContext.Vessels.Add(vessel);
                }
                else
                {
                    vessel = DataContext.Vessels.FirstOrDefault(x => x.Id == request.Id);
                    if (vessel != null)
                    {
                        request.MapPropertiesToInstance<Vessel>(vessel);
                        var measurement = new Measurement { Id = request.MeasurementId };
                        DataContext.Measurements.Attach(measurement);
                        vessel.Measurement = measurement;
                    }
                }
                DataContext.SaveChanges();
                return new SaveVesselResponse
                {
                    Id = vessel.Id,
                    IsSuccess = true,
                    Message = "Vessel has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveVesselResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }


        public DeleteVesselResponse Delete(DeleteVesselRequest request)
        {
            try
            {
                var vessel = new Vessel { Id = request.Id };
                DataContext.Vessels.Attach(vessel);
                DataContext.Vessels.Remove(vessel);
                DataContext.SaveChanges();
                return new DeleteVesselResponse
                {
                    IsSuccess = true,
                    Message = "You have been deleted this item successfully"
                };
            }
            catch (DbUpdateException e) {
                if (e.InnerException.InnerException.Message.Contains("dbo.VesselSchedules")) {
                    return new DeleteVesselResponse
                    {
                        IsSuccess = false,
                        Message = "This item is being used by Vessel Schedule"
                    };
                }
                return new DeleteVesselResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
            catch (InvalidOperationException)
            {
                return new DeleteVesselResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }


        public IEnumerable<Vessel> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Vessels.Include(x => x.Measurement).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Type.Contains(search) || x.Measurement.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Capacity)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Capacity);
                        break;
                    case "Capacity":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Capacity)
                            : data.OrderByDescending(x => x.Capacity);
                        break;
                    case "Type":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Type).ThenBy(x => x.Capacity)
                            : data.OrderByDescending(x => x.Type).ThenBy(x => x.Capacity);
                        break;
                    case "Measurement":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Measurement.Name).ThenBy(x => x.Capacity)
                            : data.OrderByDescending(x => x.Measurement.Name).ThenBy(x => x.Capacity);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public GetVesselsResponse GetVesselsForGrid(GetVesselsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetVesselsResponse
            {
                TotalRecords = totalRecords,
                Vessels = data.ToList().MapTo<GetVesselsResponse.VesselResponse>()
            };
        }
    }
}
