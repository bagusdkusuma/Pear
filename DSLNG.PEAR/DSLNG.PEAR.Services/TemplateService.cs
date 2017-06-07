
using System.Collections.Generic;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Template;
using DSLNG.PEAR.Services.Responses.Template;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class TemplateService : BaseService, ITemplateService
    {
        public TemplateService(IDataContext dataContext)
            : base(dataContext)
        {

        }

        public CreateTemplateResponse CreateTemplate(CreateTemplateRequest request)
        {
            var template = request.MapTo<DashboardTemplate>();
            var index = 0;
            foreach (var row in request.LayoutRows)
            {
                var layoutRow = new LayoutRow();
                var colIndex = 0;
                layoutRow.Index = index;
                foreach (var col in row.LayoutColumns)
                {
                    var LayoutColumn = new LayoutColumn();
                    LayoutColumn.Index = colIndex;
                    LayoutColumn.Width = col.Width;
                    LayoutColumn.HighlightPeriodeType = col.HighlightPeriodeType;
                    LayoutColumn.ColumnType = col.ColumnType;
                    if (col.ArtifactId != 0)
                    {
                        if (DataContext.Artifacts.Local.Where(x => x.Id == col.ArtifactId).FirstOrDefault() == null)
                        {
                            var artifact = new Artifact { Id = col.ArtifactId, GraphicType = "Unchanged", GraphicName = "Unchanged", HeaderTitle = "Unchanged" };
                            //DataContext.Entry(artifact).State = EntityState.Unchanged;
                            DataContext.Artifacts.Attach(artifact);
                            LayoutColumn.Artifact = artifact;
                        }
                        else
                        {
                            LayoutColumn.Artifact = DataContext.Artifacts.Local.Where(x => x.Id == col.ArtifactId).FirstOrDefault();
                        }
                    }
                    if (col.HighlightTypeId != 0) {
                        if (DataContext.SelectOptions.Local.Where(x => x.Id == col.HighlightTypeId).FirstOrDefault() == null)
                        {
                            var highlightType = new SelectOption { Id = col.HighlightTypeId };
                            DataContext.SelectOptions.Attach(highlightType);
                            LayoutColumn.HighlightType = highlightType;
                        }
                        else {
                            LayoutColumn.HighlightType = DataContext.SelectOptions.Local.Where(x => x.Id == col.HighlightTypeId).FirstOrDefault();
                        }
                    }
                    layoutRow.LayoutColumns.Add(LayoutColumn);
                    colIndex++;
                }
                template.LayoutRows.Add(layoutRow);
                index++;
            }
            DataContext.DashboardTemplates.Add(template);
            DataContext.SaveChanges();
            return new CreateTemplateResponse();
        }

        public GetTemplatesResponse GetTemplates(GetTemplatesRequest request)
        {

            int totalRecords;
            var query = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if(request.Take != - 1){
                query = query.Skip(request.Skip).Take(request.Take);
            }
            var templates = query.ToList();

            var response = new GetTemplatesResponse();
            response.Artifacts = templates.MapTo<GetTemplatesResponse.TemplateResponse>();
            response.TotalRecords = totalRecords;

            return response;
            /*if (request.OnlyCount)
            {
                return new GetTemplatesResponse { Count = DataContext.DashboardTemplates.Count() };
            }
            else
            {
                return new GetTemplatesResponse
                {
                    Artifacts = DataContext.DashboardTemplates.OrderBy(x => x.Id).Skip(request.Skip).Take(request.Take)
                                    .ToList().MapTo<GetTemplatesResponse.TemplateResponse>()
                };
            }*/
        }

        public GetTemplateResponse GetTemplate(GetTemplateRequest request)
        {
            return DataContext.DashboardTemplates.Include(x => x.LayoutRows)
                .Include(x => x.LayoutRows.Select(y => y.LayoutColumns))
                .Include(x => x.LayoutRows.Select(y => y.LayoutColumns.Select(z => z.Artifact)))
                .Include(x => x.LayoutRows.Select(y => y.LayoutColumns.Select(z => z.HighlightType)))
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetTemplateResponse>();
        }

        public UpdateTemplateResponse UpdateTemplate(UpdateTemplateRequest request)
        {
            var template = DataContext.DashboardTemplates
                .Include(x => x.LayoutRows)
                .Include(x => x.LayoutRows.Select(y => y.LayoutColumns))
                .Single(x => x.Id == request.Id);
            template.Name = request.Name;
            template.RefershTime = request.RefershTime;
            template.Remark = request.Remark;
            template.IsActive = request.IsActive;
            foreach (var row in template.LayoutRows.ToList())
            {
                foreach (var column in row.LayoutColumns.ToList())
                {
                    DataContext.LayoutColumns.Remove(column);
                }
                DataContext.LayoutRows.Remove(row);
            }

            var index = 0;
            foreach (var row in request.LayoutRows)
            {
                var layoutRow = new LayoutRow();
                var colIndex = 0;
                layoutRow.Index = index;
                foreach (var col in row.LayoutColumns)
                {
                    var layoutColumn = new LayoutColumn();
                    layoutColumn.Index = colIndex;
                    layoutColumn.Width = col.Width;
                    layoutColumn.HighlightPeriodeType = col.HighlightPeriodeType;
                    layoutColumn.ColumnType = col.ColumnType;
                    if (col.ArtifactId != 0)
                    {
                        if (DataContext.Artifacts.Local.FirstOrDefault(x => x.Id == col.ArtifactId) == null)
                        {
                            var artifact = new Artifact { Id = col.ArtifactId, GraphicType = "Unchanged", GraphicName = "Unchanged", HeaderTitle = "Unchanged" };
                            //DataContext.Entry(artifact).State = EntityState.Unchanged;
                            DataContext.Artifacts.Attach(artifact);
                            layoutColumn.Artifact = artifact;
                        }
                        else
                        {
                            layoutColumn.Artifact = DataContext.Artifacts.Local.FirstOrDefault(x => x.Id == col.ArtifactId);
                        }
                    }
                    if (col.HighlightTypeId != 0)
                    {
                        if (DataContext.SelectOptions.Local.Where(x => x.Id == col.HighlightTypeId).FirstOrDefault() == null)
                        {
                            var highlightType = new SelectOption { Id = col.HighlightTypeId };
                            DataContext.SelectOptions.Attach(highlightType);
                            layoutColumn.HighlightType = highlightType;
                        }
                        else
                        {
                            layoutColumn.HighlightType = DataContext.SelectOptions.Local.Where(x => x.Id == col.HighlightTypeId).FirstOrDefault();
                        }
                    }
                    layoutRow.LayoutColumns.Add(layoutColumn);
                    colIndex++;
                }
                template.LayoutRows.Add(layoutRow);
                index++;
            }
            DataContext.DashboardTemplates.Attach(template);
            DataContext.Entry(template).State = EntityState.Modified;
            DataContext.SaveChanges();
            
            return new UpdateTemplateResponse();
        }

        private IEnumerable<DashboardTemplate> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.DashboardTemplates.AsQueryable();
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
                                   ? data.OrderBy(x => x.Name)
                                   : data.OrderByDescending(x => x.Name);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Name)
                            : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Name);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }


        public DeleteTemplateResponse DeleteTemplate(DeleteTemplateRequest request)
        {
           try
           {
               var template = DataContext.DashboardTemplates.FirstOrDefault(x => x.Id == request.Id);
               if (template != null)
               {
                   DataContext.DashboardTemplates.Attach(template);
                   DataContext.DashboardTemplates.Remove(template);
                   DataContext.SaveChanges();
               }
               return new DeleteTemplateResponse
               {
                   IsSuccess = true,
                   Message = "Template has been deleted"
               };
           }
            catch(DbUpdateException exception)
           {
               return new DeleteTemplateResponse
               {
                   IsSuccess = false,
                   Message = exception.Message
               };
           }
        }
    }
}
