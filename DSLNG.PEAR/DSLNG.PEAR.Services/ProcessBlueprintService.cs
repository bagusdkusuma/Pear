using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using DSLNG.PEAR.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Services
{
    public class ProcessBlueprintService : BaseService, IProcessBlueprintService
    {
        public ProcessBlueprintService(IDataContext dataContext):base(dataContext)
        {

        }
        public GetProcessBlueprintsResponse Gets(GetProcessBlueprintsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetProcessBlueprintsResponse
            {
                TotalRecords = totalRecords,
                ProcessBlueprints = data.ToList().MapTo<GetProcessBlueprintsResponse.ProcessBlueprint>()
            };
        }

        private IEnumerable<ProcessBlueprint> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.ProcessBlueprints.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.ParentId).ThenBy(x => x.LastWriteTime)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.LastWriteTime);
                        break;
                    default:
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Id).ThenBy(x => x.ParentId).ThenBy(x => x.LastWriteTime)
                            : data.OrderBy(x => x.ParentId).ThenBy(x => x.Id).ThenBy(x => x.LastWriteTime);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
            
        }

        public GetProcessBlueprintResponse Get(GetProcessBlueprintRequest request)
        {
            var data = DataContext.ProcessBlueprints.FirstOrDefault(x => x.Id == request.Id);
            return data.MapTo<GetProcessBlueprintResponse>();
        }

        public GetProcessBlueprintResponse Save(SaveProcessBlueprintRequest request)
        {
            throw new NotImplementedException();
        }


        public GetProcessBlueprintResponse Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public GetProcessBlueprintsResponse All()
        {
            var data = DataContext.ProcessBlueprints.ToList();
            return new GetProcessBlueprintsResponse {
                TotalRecords = data.Count(),
                ProcessBlueprints = data.ToList().MapTo<GetProcessBlueprintsResponse.ProcessBlueprint>()
            };
        }
    }
}
