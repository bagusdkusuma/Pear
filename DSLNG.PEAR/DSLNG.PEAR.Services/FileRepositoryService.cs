﻿using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Files;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Files;
using DSLNG.PEAR.Data.Entities.Files;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class FileRepositoryService : BaseService, IFileRepositoryService
    {
        public FileRepositoryService(IDataContext context):base(context)
        {

        }

        public BaseResponse Delete(int Id)
        {
            try
            {
                var file = DataContext.FileRepositories.Find(Id);
                DataContext.Entry(file).State = EntityState.Deleted;
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "File Successfully Deleted"
                };
            }
            catch (DbUpdateException u)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = u.Message
                };
            }
            catch(InvalidOperationException i)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = i.Message
                };
            }
        }

        public GetFileRepositoryResponse GetFile(GetFileRequest request)
        {
            var response = new GetFileRepositoryResponse();
            try
            {
                var data = DataContext.FileRepositories.Find(request.Id);
                response = data.MapTo<GetFileRepositoryResponse>();
                response.IsSuccess = true;
            }
            catch (InvalidOperationException e)
            {

                response.Message = e.Message;
            }
            return response;
        }

        public GetFilesRepositoryResponse GetFiles(GetFilesRequest request)
        {
            int totalRecord = 0;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecord);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new GetFilesRepositoryResponse
            {
                TotalRecords = totalRecord,
                FileRepositories = data.ToList().MapTo<GetFilesRepositoryResponse.FileRepository>()
            };
        }

        private IEnumerable<FileRepository> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.FileRepositories.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Summary.Contains(search));
            }
            if (sortingDictionary != null)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "Name":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Name).ThenBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime)
                                : data.OrderByDescending(x => x.Name).ThenBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime);
                            break;
                        case "Year":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Year).ThenBy(x => x.Id).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime)
                                : data.OrderByDescending(x => x.Year).ThenBy(x => x.Id).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime);
                            break;
                        case "Month":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Month).ThenBy(x => x.Year).ThenBy(x => x.Id).ThenBy(x => x.LastWriteTime)
                                : data.OrderByDescending(x => x.Month).ThenBy(x => x.Year).ThenBy(x => x.Id).ThenBy(x => x.LastWriteTime);
                            break;
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Id).ThenBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime)
                                : data.OrderByDescending(x => x.Id).ThenBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.LastWriteTime);
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Id).ThenBy(x => x.LastWriteTime);
            }
            TotalRecords = data.Count();
            return data;
        }

        public BaseResponse Save(SaveFileRepositoryRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var file = new FileRepository();
                var user = DataContext.Users.Find(request.UserId);
                file.Year = request.Year;
                file.Month = request.Month;
                file.Name = request.Name;
                file.Summary = request.Summary;
                file.LastWriteTime = request.LastWriteTime;
                file.Data = request.Data;
                if (request.Id > 0)
                {
                    file.UpdatedDate = DateTime.Now;
                    file.UpdatedBy = user;
                    DataContext.Entry(file).State = EntityState.Modified;
                }
                else
                {
                    file.CreatedDate = DateTime.Now;
                    file.CreatedBy = user;
                    DataContext.FileRepositories.Add(file);
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "File Successfully Saved";
            }
            catch (InvalidOperationException inval)
            {
                response.Message = inval.Message;
            }
            catch (DbUpdateException update)
            {
                response.Message = update.Message;
            }
            return response;
        }
    }
}
