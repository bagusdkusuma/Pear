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

        public AuditTrailsResponse GetAuditTrails(GetAuditTrailsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
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

        private IEnumerable<AuditTrail> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.AuditTrails.Include(x => x.User).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.TableName.Contains(search) || x.Action.Contains(search) || x.ActionName.Contains(search) 
                || x.ControllerName.Contains(search) || x.NewValue.Contains(search) || x.OldValue.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "User":
                        data = sortOrder.Value == SortOrder.Ascending 
                            ? data.OrderBy(x => x.User.Username) 
                            : data.OrderByDescending(x => x.User.Username);
                        break;
                    case "TableName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.TableName)
                            : data.OrderByDescending(x => x.TableName);
                        break;
                    case "Controller":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ControllerName)
                            : data.OrderByDescending(x => x.ControllerName);
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
