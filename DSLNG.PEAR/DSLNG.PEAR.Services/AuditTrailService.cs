using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using DSLNG.PEAR.Data.Entities;
using System.Data.SqlClient;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Services
{
    public class AuditTrailService : BaseService, IAuditTrailService
    {
        public AuditTrailService(IDataContext dataContext) : base(dataContext)
        {
        }

        public AuditTrailsResponse GetAuditTrail(GetAuditTrailRequest request)
        {
            var data = DataContext.AuditTrails.Where(x => x.RecordId == request.RecordId);
            return new AuditTrailsResponse
            {
                TotalRecords = data.Count(),
                AuditTrails = data.ToList().MapTo<AuditTrailsResponse.AuditTrail>()
            };
        }

        public AuditTrailsResponse GetAuditTrailDetails(int recordId)
        {
            var response = new AuditTrailsResponse();
            try
            {
                var auditDetails = DataContext.AuditTrails.Where(x => x.RecordId == recordId).OrderByDescending(x => x.UpdateDate).ToList();
                response.AuditTrails = auditDetails.MapTo<AuditTrailsResponse.AuditTrail>();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public AuditTrailsResponse GetAuditTrails(GetAuditTrailsRequest request)
        {
            int totalRecords;
            var data = SortData(request, request.SortingDictionary, out totalRecords);
            if(request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new AuditTrailsResponse
            {
                TotalRecords = totalRecords,
                AuditTrails = data.ToList().MapTo<AuditTrailsResponse.AuditTrail>()
            };
        }

        private IEnumerable<AuditTrail> SortData(GetAuditTrailsRequest request, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.AuditTrails.AsQueryable();
            if (!string.IsNullOrEmpty(request.Search) && !string.IsNullOrWhiteSpace(request.Search))
            {
                data = data.Where(x => x.TableName.Contains(request.Search) || x.Action.Contains(request.Search) || x.ActionName.Contains(request.Search) 
                || x.ControllerName.Contains(request.Search) || x.NewValue.Contains(request.Search) || x.OldValue.Contains(request.Search));
            }

            if(request.StartDate != null)
            {
                data = data.Where(x => x.UpdateDate >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                data = data.Where(x => x.UpdateDate <= request.EndDate);
            }

            data = data.GroupBy(x => x.RecordId).Select(y => y.OrderByDescending(x => x.UpdateDate).FirstOrDefault())
                .OrderByDescending(x => x.UpdateDate).Include(x => x.User);

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "User":
                        data = sortOrder.Value == SortOrder.Ascending 
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.User.Username) 
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.User.Username);
                        break;
                    case "TableName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.TableName)
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.TableName);
                        break;
                    case "Controller":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.User.Username)
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.ControllerName);
                        break;
                    default:
                        data = data.OrderByDescending(x => x.UpdateDate);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
        }
    }
}
