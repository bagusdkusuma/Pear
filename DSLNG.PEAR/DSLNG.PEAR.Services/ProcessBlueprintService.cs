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
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Services.Responses;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class ProcessBlueprintService : BaseService, IProcessBlueprintService
    {
        public ProcessBlueprintService(IDataContext dataContext)
            : base(dataContext)
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
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).AsQueryable();
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
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).FirstOrDefault(x => x.Id == request.Id);
            return data.MapTo<GetProcessBlueprintResponse>();
        }

        public SaveProcessBlueprintResponse Save(SaveProcessBlueprintRequest request)
        {
            var response = new SaveProcessBlueprintResponse();
            try
            {
                var proses = new ProcessBlueprint();
                var user = DataContext.Users.Single(x => x.Id == request.UserId);
                if (request.Id > 0)
                {
                    proses = DataContext.ProcessBlueprints.Single(x => x.Id == request.Id);
                    proses.Name = request.Name;
                    proses.ParentId = request.ParentId;
                    proses.IsFolder = request.IsFolder;
                    proses.LastWriteTime = request.LastWriteTime;
                    proses.Data = request.Data;
                    proses.UpdatedBy = user;
                    DataContext.Entry(proses).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    proses.Name = request.Name;
                    proses.IsFolder = request.IsFolder;
                    proses.ParentId = request.ParentId;
                    proses.Data = request.Data;
                    proses.LastWriteTime = request.LastWriteTime;
                    proses.CreatedBy = user;
                    DataContext.ProcessBlueprints.Add(proses);
                }

                DataContext.SaveChanges();
                DataContext.Entry(proses).GetDatabaseValues();
                response.Id = proses.Id;
                response.IsSuccess = true;
                response.Message = "File Manager Successfully Saved";
            }
            catch (DbUpdateException ex)
            {

                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public BaseResponse Delete(int Id)
        {
            var response = new BaseResponse();
            try
            {
                var prosess = DataContext.ProcessBlueprints.Single(x => x.Id == Id);
                DataContext.ProcessBlueprints.Remove(prosess);
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public GetProcessBlueprintsResponse All()
        {
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).ToList();
            return new GetProcessBlueprintsResponse
            {
                TotalRecords = data.Count(),
                ProcessBlueprints = data.ToList().MapTo<GetProcessBlueprintsResponse.ProcessBlueprint>()
            };
        }
    }
}
