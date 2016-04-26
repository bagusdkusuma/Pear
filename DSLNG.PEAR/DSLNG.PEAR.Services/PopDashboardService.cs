using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PopDashboard;
using DSLNG.PEAR.Services.Responses.PopDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Pop;
using System.Data.SqlClient;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class PopDashboardService : BaseService, IPopDashboardService
    {
        public PopDashboardService(IDataContext dataContext) : base(dataContext) { }



        public GetPopDashboardsResponse GetPopDashboards(GetPopDashboardsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetPopDashboardsResponse
            {
                TotalRecords = totalRecords,
                PopDashboards = data.ToList().MapTo<GetPopDashboardsResponse.PopDashboard>()
            };


        }

        public IEnumerable<PopDashboard> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.PopDashboards.Include(x => x.Attachments).Include(x => x.PopInformations).Include(x => x.Signatures).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Number.Contains(search) || x.Title.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Title":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Title)
                            : data.OrderByDescending(x => x.Title);
                        break;

                    case "Number":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Number)
                            : data.OrderByDescending(x => x.Number);
                        break;
                    case "Status":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Status)
                            : data.OrderByDescending(x => x.Status);
                        break;
                    case "StructureOwner":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.StructureOwner)
                            : data.OrderByDescending(x => x.StructureOwner);
                        break;
                }
            }


            TotalRecords = data.Count();
            return data;
        }



        public SavePopDashboardResponse SavePopDashboard(SavePopDashboardRequest request)
        {
            var popDashboard = request.MapTo<PopDashboard>();
            if (request.Id == 0)
            {
                foreach (var attachment in request.AttachmentFiles)
                {
                    popDashboard.Attachments.Add(new PopDashboardAttachment
                    {
                        Alias = attachment.Alias,
                        Filename = attachment.FileName,
                        Type = attachment.Type
                    });
                }
                DataContext.PopDashboards.Add(popDashboard);
            }
            else
            {
                popDashboard = DataContext.PopDashboards
                    .Include(x => x.Attachments)
                    .FirstOrDefault(x => x.Id == request.Id);

                foreach (var attachment in popDashboard.Attachments.ToList()) { 
                    if(request.AttachmentFiles.All(x => x.Id != attachment.Id)){
                         popDashboard.Attachments.Remove(attachment);
                    }
                }

                foreach (var attachmentReq in request.AttachmentFiles)
                {
                    var existingAttachment = popDashboard.Attachments.SingleOrDefault(c => c.Id == attachmentReq.Id);

                    if (existingAttachment != null && existingAttachment.Id != 0)
                    {
                        existingAttachment.Alias = attachmentReq.Alias;
                        existingAttachment.Filename = string.IsNullOrEmpty(attachmentReq.FileName) ? existingAttachment.Filename : attachmentReq.FileName;
                        existingAttachment.Type = string.IsNullOrEmpty(attachmentReq.Type) ? existingAttachment.Type : attachmentReq.Type;
                    }
                    else
                    {
                        var newAttachment = new PopDashboardAttachment()
                            {
                                Alias = attachmentReq.Alias,
                                Type = attachmentReq.Type,
                                Filename = attachmentReq.FileName
                            };
                        popDashboard.Attachments.Add(newAttachment);
                    }
                }


                request.MapPropertiesToInstance<PopDashboard>(popDashboard);
            }

            DataContext.SaveChanges();
            return new SavePopDashboardResponse
            {
                IsSuccess = true,
                Message = "Project  has been saved successfully!"
            };
        }



        public GetPopDashboardResponse GetPopDashboard(GetPopDashboardRequest request)
        {
            return DataContext.PopDashboards.Where(x => x.Id == request.Id)
                .Include(x => x.Attachments)
                .Include(x => x.PopInformations)
                .Include(x => x.Signatures)
                .Include(x => x.Signatures.Select(y => y.User))
                .FirstOrDefault().MapTo<GetPopDashboardResponse>();
        }


        public DeletePopDashboardResponse DeletePopDashboard(int request)
        {
            var popDashboard = DataContext.PopDashboards
                .Include(x => x.Attachments)
                .FirstOrDefault(x => x.Id == request);
            foreach(var attchment in popDashboard.Attachments.ToList()){
                popDashboard.Attachments.Remove(attchment);
            }
            DataContext.PopDashboards.Remove(popDashboard);
            DataContext.SaveChanges();
            return new DeletePopDashboardResponse
            {
                IsSuccess = true,
                Message = "Project has been Deleted!"
            };
        }
    }
}
