using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Conversion;
using DSLNG.PEAR.Services.Responses.Conversion;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class ConversionService : BaseService, IConversionService
    {
        public ConversionService(IDataContext dataContext) : base (dataContext)
        {

        }

        public GetConversionResponse GetConversion(GetConversionRequest request){
            try
            {
                var conversion = DataContext.Conversions.Include(f => f.From).Include(t => t.To).First(x => x.Id == request.Id);
                //var conversion = DataContext.Conversions.First(x => x.Id == request.Id);
                var response = new GetConversionResponse();
                response = conversion.MapTo<GetConversionResponse>();

                return response;
            }catch (System.InvalidOperationException x){
                return new GetConversionResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }
        public GetConversionsResponse GetConversions(GetConversionsRequest request){
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if(request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetConversionsResponse
            {
                TotalRecords = totalRecords,
                Conversions = data.ToList().MapTo<GetConversionsResponse.Conversion>()
            };

            //var conversions = DataContext.Conversions.Include(f => f.From).Include(t => t.To).ToList();
            //var response = new GetConversionsResponse();
            //response.Conversions = conversions.MapTo<GetConversionsResponse.Conversion>();

            //return response;
        }
        public CreateConversionResponse Create(CreateConversionRequest request)
        {
            var response = new CreateConversionResponse();
            try
            {
                var conversion = request.MapTo<Conversion>();
                conversion.From = DataContext.Measurements.FirstOrDefault(x => x.Id == request.MeasurementFrom);
                conversion.To = DataContext.Measurements.FirstOrDefault(x => x.Id == request.MeasurementTo);

                DataContext.Conversions.Add(conversion);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Conversion item has been added successfully";
            }
            catch (DbUpdateException exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public UpdateConversionResponse Update(UpdateConversionRequest request){
            var response = new UpdateConversionResponse();
            try
            {
                var conversion = request.MapTo<Conversion>();
                conversion.From = DataContext.Measurements.FirstOrDefault(x => x.Id == request.MeasurementFrom);
                conversion.To = DataContext.Measurements.FirstOrDefault(x => x.Id == request.MeasurementTo);
                DataContext.Conversions.Attach(conversion);
                DataContext.Entry(conversion).State = EntityState.Modified;
                DataContext.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Conversion item has been updated successfully";
            }
            catch (DbUpdateException exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public DeleteConversionResponse Delete(int Id)
        {
            var response = new DeleteConversionResponse();

            try
            {
                var conversion = new Conversion { Id = Id};
                DataContext.Conversions.Attach(conversion);
                DataContext.Entry(conversion).State = EntityState.Deleted;
                DataContext.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Conversion item has been deleted successfully";
            }
            catch (DbUpdateException exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public IEnumerable<Conversion> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Conversions.Include(x => x.From).Include(x => x.To).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.From.Name.Contains(search) || x.To.Name.Contains(search));
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
                    case "FromName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.From.Name)
                            : data.OrderByDescending(x => x.From.Name);
                        break;
                    case "ToName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.To.Name)
                            : data.OrderByDescending(x => x.To.Name);
                        break;
                    case "Value":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Value)
                            : data.OrderByDescending(x => x.Value);
                        break;
                    case "IsReverse":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsReverse)
                            : data.OrderByDescending(x => x.IsReverse);
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
    }
}
