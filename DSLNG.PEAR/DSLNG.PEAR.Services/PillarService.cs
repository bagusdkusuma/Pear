using System.Data.SqlClient;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Pillar;
using DSLNG.PEAR.Services.Responses.Pillar;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class PillarService : BaseService, IPillarService
    {
        public PillarService(IDataContext dataContext): base(dataContext)
        {

        }
        public GetPillarResponse GetPillar(GetPillarRequest request)
        {
            try
            {
                var pillar = DataContext.Pillars.First(x => x.Id == request.Id);
                var response = pillar.MapTo<GetPillarResponse>(); 

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetPillarResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public GetPillarsResponse GetPillars(GetPillarsRequest request)
        {
            int totalRecords;
            var pillars = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                pillars = pillars.Skip(request.Skip).Take(request.Take);
            }

            var response = new GetPillarsResponse();
            response.Pillars = pillars.ToList().MapTo<GetPillarsResponse.Pillar>();
            response.TotalRecords = totalRecords;

            return response;
        }

        public CreatePillarResponse Create(CreatePillarRequest request)
        {
            var response = new CreatePillarResponse();
            try
            {
                var pillar = request.MapTo<Pillar>();
                DataContext.Pillars.Add(pillar);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Pillar item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdatePillarResponse Update(UpdatePillarRequest request)
        {
            var response = new UpdatePillarResponse();
            try
            {
                var pillar = request.MapTo<Pillar>();
                DataContext.Pillars.Attach(pillar);
                DataContext.Entry(pillar).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Pillar item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeletePillarResponse Delete(int id)
        {
            var response = new DeletePillarResponse();
            try
            {
                var pillar = new Pillar { Id = id };
                DataContext.Pillars.Attach(pillar);
                DataContext.Entry(pillar).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Pillar item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        private IEnumerable<Pillar> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Pillars.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Code":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Code).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Code).ThenBy(x => x.Order);
                        break;
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                                   : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                        break;
                    default:
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Order)
                                   : data.OrderByDescending(x => x.Order);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }
    }
}
