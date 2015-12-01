using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Responses.Periode;
using DSLNG.PEAR.Services.Requests.Periode;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class PeriodeService : BaseService, IPeriodeService
    {
        public PeriodeService(IDataContext dataContext)
            : base(dataContext)
        {

        }

        public GetPeriodeResponse GetPeriode(GetPeriodeRequest request)
        {
            try
            {
                var periode = DataContext.Periodes.First(x => x.Id == request.Id);
                var response = periode.MapTo<GetPeriodeResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetPeriodeResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }


        public GetPeriodesResponse GetPeriodes(GetPeriodesRequest request)
        {
            var response = new GetPeriodesResponse();
            var periodes = DataContext.Periodes.ToList();
            response.Periodes = periodes.MapTo<GetPeriodesResponse.Periode>();
            return response;
        }

        public CreatePeriodeResponse Create(CreatePeriodeRequest request)
        {
            var response = new CreatePeriodeResponse();
            try
            {
                var periode = request.MapTo<Periode>();
                DataContext.Periodes.Add(periode);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Periode item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public UpdatePeriodeResponse Update(UpdatePeriodeRequest request)
        {
            var response = new UpdatePeriodeResponse();
            try
            {
                var periode = request.MapTo<Periode>();
                DataContext.Periodes.Attach(periode);
                DataContext.Entry(periode).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Periode item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public DeletePeriodeResponse Delete(int id)
        {
            var response = new DeletePeriodeResponse();
            try
            {
                var periode = new Periode { Id = id };
                DataContext.Periodes.Attach(periode);
                DataContext.Entry(periode).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Periode item has been Deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }


        public IEnumerable<Periode> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Periodes.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                //data = data.Where(x => x.Nam);
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
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public GetPeriodesResponse GetPeriodesForGrid(GetPeriodesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetPeriodesResponse
            {
                TotalRecords = totalRecords,
                Periodes = data.ToList().MapTo<GetPeriodesResponse.Periode>()
            };
        }
    }
}
