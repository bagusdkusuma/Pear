using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Responses.Select;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class SelectService : BaseService, ISelectService
    {
        public SelectService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public CreateSelectResponse Create(CreateSelectRequest request)
        {
            var response = new CreateSelectResponse();
            try
            {
                var select = request.MapTo<Select>();
                DataContext.Selects.Add(select);
                if (request.ParentId != 0)
                {
                    var parent = new Select { Id = request.ParentId };
                    DataContext.Selects.Attach(parent);
                    select.Parent = parent;
                }
                if (request.ParentOptionId != 0)
                {
                    var parentOption = new SelectOption { Id = request.ParentOptionId };
                    DataContext.SelectOptions.Attach(parentOption);
                    select.ParentOption = parentOption;
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Select has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateSelectResponse Update(UpdateSelectRequest request)
        {
            var response = new UpdateSelectResponse();
            try
            {
                var select = DataContext.Selects.Where(p => p.Id == request.Id)
                                        .Include(p => p.Options)
                                        .Include(p => p.Parent)
                                        .Include(p => p.ParentOption)
                                        .Single();

                DataContext.Entry(select).CurrentValues.SetValues(request);

                foreach (var option in select.Options.ToList())
                {
                    if (request.Options.All(c => c.Id != option.Id))
                        DataContext.SelectOptions.Remove(option);
                }

                foreach (var option in request.Options)
                {
                    var existingOption = select.Options.SingleOrDefault(c => c.Id == option.Id);

                    if (existingOption != null && option.Id != 0)
                    {
                        DataContext.Entry(existingOption).CurrentValues.SetValues(option);
                    }
                    else
                    {
                        var newOption = new SelectOption()
                            {
                                Text = option.Text,
                                Value = option.Value
                            };
                        select.Options.Add(newOption);
                    }
                }

                if (request.ParentId != 0 && (select.Parent == null || select.Parent.Id != request.ParentId))
                {
                    var parent = new Select { Id = request.ParentId };
                    DataContext.Selects.Attach(parent);
                    select.Parent = parent;
                    var parentOption = new SelectOption { Id = request.ParentOptionId };
                    DataContext.SelectOptions.Attach(parentOption);
                    select.ParentOption = parentOption;
                }
                else if (request.ParentId == 0)
                {
                    select.Parent = null;
                    select.ParentOption = null;
                }

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Select has been updated successfully";

            }
            catch (DbUpdateException exception)
            {
                response.Message = exception.Message;
            }
            catch (ArgumentNullException exception)
            {
                response.Message = exception.Message;
            }
            catch (InvalidOperationException exception)
            {
                response.Message = exception.Message;
            }
            return response;
        }

        public DeleteSelectResponse Delete(int id)
        {
            var response = new DeleteSelectResponse();
            try
            {
                var select = DataContext.Selects
                    .Include(x => x.Options)
                    .Single(x => x.Id == id);
                foreach (var selectOption in select.Options.ToList())
                {
                    DataContext.SelectOptions.Remove(selectOption);
                }

                DataContext.Selects.Remove(select);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Select item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public GetSelectResponse GetSelect(GetSelectRequest request)
        {
            var response = new GetSelectResponse();
            try
            {
                var query = DataContext.Selects.Include(x => x.Options);
                if (request.Id != 0)
                {
                    query = query.Where(x => x.Id == request.Id);
                }
                else if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name == request.Name);
                }
                else if (!string.IsNullOrEmpty(request.ParentName) && request.ParentOptionId != 0)
                {
                    query = query.Where(x => x.Parent.Name == request.ParentName && x.ParentOption.Id == request.ParentOptionId);
                }

                var select = query.Include(x => x.Parent).Include(x => x.Parent.Options).FirstOrDefault();
                response = select.MapTo<GetSelectResponse>();
                if (select.Parent != null)
                {
                    response.ParentId = select.Parent.Id;
                    response.ParentOptions = select.Parent.Options.MapTo<GetSelectResponse.SelectOptionResponse>();
                }
                if (select.ParentOption != null)
                {
                    response.ParentOptionId = select.ParentOption.Id;
                }
                response.IsSuccess = true;
                response.Message = "Success get select with id=" + request.Id;
            }
            catch (ArgumentNullException nullException)
            {
                response.Message = nullException.Message;
            }

            return response;
        }

        public GetSelectsResponse GetSelects(GetSelectsRequest request)
        {
            List<Select> selects;
            if (request.Take != 0)
            {
                selects = DataContext.Selects
                    .Include(x => x.Options)
                    .OrderBy(x => x.Id).Skip(request.Skip).Take(request.Take).ToList();
            }
            else
            {
                selects = DataContext.Selects
                    .Include(x => x.Options)
                    .OrderByDescending(x => x.Id).ToList();
            }
            var response = new GetSelectsResponse();
            response.Selects = selects.MapTo<GetSelectsResponse.Select>();
            return response;
        }



        public IEnumerable<Select> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Selects.Include(x => x.Options).Include(x => x.Parent).Include(x => x.ParentOption).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Parent.Name.Contains(search) || x.ParentOption.Text.Contains(search));
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
                    case "Parent":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Parent.Name)
                            : data.OrderByDescending(x => x.Parent.Name);
                        break;
                    case "ParentOption":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.ParentOption.Text)
                            : data.OrderByDescending(x => x.ParentOption.Text);
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


        public GetSelectsResponse GetSelectsForGrid(GetSelectsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetSelectsResponse
            {
                TotalRecords = totalRecords,
                Selects = data.ToList().MapTo<GetSelectsResponse.Select>()
            };
        }

        public IList<Dropdown> GetHighlightTypesDropdown()
        {
            var dropdowns =
                DataContext.SelectOptions.Where(x => x.IsActive && x.Select.Name == "highlight-types")
                           .ToList();
            return dropdowns.Select(x => new Dropdown() { Text = x.Text, Value = x.Id.ToString() }).ToList();
        }
    }
}
