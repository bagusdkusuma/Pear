
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.NLS;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Common.Extensions;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetNLSResponse>();
        }

        public GetNLSListResponse GetNLSList(GetNLSListRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetNLSListResponse { Count = DataContext.NextLoadingSchedules.Count() };
            }
            else if (request.VesselScheduleId != 0) {
                var query = DataContext.NextLoadingSchedules.Include(x => x.VesselSchedule).Include(x => x.VesselSchedule.Vessel);
                query = query.Where(x => x.VesselSchedule.Id == request.VesselScheduleId).OrderByDescending(x => x.CreatedAt);
                return new GetNLSListResponse
                {
                    NLSList = query.ToList().MapTo<GetNLSListResponse.NLSResponse>()
                }; 
            }
            else
            {
                var query = DataContext.NextLoadingSchedules.Include(x => x.VesselSchedule).Include(x => x.VesselSchedule.Vessel);
                if (!string.IsNullOrEmpty(request.Term))
                {
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
                var response = new SaveNLSResponse
                {
                    IsSuccess = true,
                    Message = "Next Loading Schedule has been saved"
                };
                if (request.DerTransactionDate.HasValue) {
                    var nls = DataContext.NextLoadingSchedules.Where(x => x.VesselSchedule.Id == request.VesselScheduleId && x.CreatedAt <= request.DerTransactionDate)
                   .OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    if (nls != null) {
                        response.RemarkDate = nls.CreatedAt.ToString("dd-MM-yyyy");
                        response.Remark = nls.Remark;
                    }
                }
                return response;
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


        public DeleteNLSResponse Delete(DeleteNLSRequest request)
        {
            try
            {
                var nextLoadingSchedule = new NextLoadingSchedule { Id = request.Id };
                DataContext.NextLoadingSchedules.Attach(nextLoadingSchedule);
                DataContext.NextLoadingSchedules.Remove(nextLoadingSchedule);
                DataContext.SaveChanges();
                return new DeleteNLSResponse
                {
                    IsSuccess = true,
                    Message = "You have been deleted this item successfully"
                };
            }
            catch (InvalidOperationException exception) {
                return new DeleteNLSResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }


        public IEnumerable<NextLoadingSchedule> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.NextLoadingSchedules.Include(x => x.VesselSchedule).Include(x => x.VesselSchedule.Vessel).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.VesselSchedule.Name.Contains(search) || x.VesselSchedule.Vessel.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Vessel":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.VesselSchedule.Vessel.Name)
                            : data.OrderByDescending(x => x.VesselSchedule.Vessel.Name);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }

        public GetNLSListResponse GetNLSListForGrid(GetNLSListRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetNLSListResponse
            {
                TotalRecords = totalRecords,
                NLSList = data.ToList().MapTo<GetNLSListResponse.NLSResponse>()
            };
        }


    }
}
