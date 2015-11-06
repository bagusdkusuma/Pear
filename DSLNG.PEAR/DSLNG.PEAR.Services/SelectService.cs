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
                if (request.ParentId != 0) {
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

                    if (existingOption != null)
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
                if (request.ParentId != 0)
                {
                    var parent = new Select { Id = request.ParentId };
                    DataContext.Selects.Attach(parent);
                    select.Parent = parent;
                     var parentOption = new SelectOption { Id = request.ParentOptionId };
                    DataContext.SelectOptions.Attach(parentOption);
                    select.ParentOption = parentOption;
                }
                else {
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
                if (request.Id != 0) {
                    query = query.Where(x => x.Id == request.Id);
                }
                else if (!string.IsNullOrEmpty(request.Name)) {
                    query = query.Where(x => x.Name == request.Name);
                }
                else if (!string.IsNullOrEmpty(request.ParentName) && !string.IsNullOrEmpty(request.ParentOptionValue)) {
                    query = query.Where(x => x.Parent.Name == request.ParentName && x.ParentOption.Value == request.ParentOptionValue);
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
    }
}
